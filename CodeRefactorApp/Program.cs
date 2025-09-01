using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeRefactorTool
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter the path to the solution (.sln) or project (.csproj):");
            string path = Console.ReadLine() ?? "";

            if (!File.Exists(path))
            {
                Console.WriteLine("File not found.");
                return;
            }

            using var workspace = MSBuildWorkspace.Create();
            Solution solution = path.EndsWith(".sln")
                ? await workspace.OpenSolutionAsync(path)
                : (await workspace.OpenProjectAsync(path)).Solution;

            // -----------------------------
            // 1️⃣ Collect all async methods to rename
            // -----------------------------
            var asyncMethods = solution.Projects
                .SelectMany(p => p.Documents)
                .SelectMany(d =>
                {
                    var root = d.GetSyntaxRootAsync().Result;
                    var model = d.GetSemanticModelAsync().Result;
                    if (root == null || model == null) return Enumerable.Empty<(IMethodSymbol, Document)>();

                    var methods = root.DescendantNodes()
                        .OfType<MethodDeclarationSyntax>()
                        .Where(m => m.Modifiers.Any(SyntaxKind.AsyncKeyword) && !m.Identifier.Text.EndsWith("Async"));

                    return methods.Select(m => (model.GetDeclaredSymbol(m), d))
                                  .Where(t => t.Item1 != null)!;
                }).ToList();

            // -----------------------------
            // 2️⃣ Rename async methods across the solution
            // -----------------------------
            foreach (var (symbol, document) in asyncMethods)
            {
                string newName = symbol.Name + "Async";
                solution = await Renamer.RenameSymbolAsync(solution, symbol, newName, solution.Workspace.Options);
            }

            // -----------------------------
            // 3️⃣ Apply class/interface renames and blank lines per document
            // -----------------------------
            foreach (var project in solution.Projects)
            {
                foreach (var document in project.Documents)
                {
                    var root = await document.GetSyntaxRootAsync();
                    if (root == null) continue;

                    var rewriter = new VmDtoAndBlankLineRewriter();
                    var newRoot = rewriter.Visit(root);

                    if (newRoot != root)
                    {
                        solution = solution.WithDocumentSyntaxRoot(document.Id, newRoot);
                    }
                }
            }

            Console.WriteLine("Applying changes...");
            workspace.TryApplyChanges(solution);
            Console.WriteLine("Done.");
        }
    }
}
