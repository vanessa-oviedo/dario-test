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

            // 1️⃣ Collect all async method symbols
            var asyncMethods = solution.Projects
                .SelectMany(p => p.Documents)
                .SelectMany(d =>
                {
                    var root = d.GetSyntaxRootAsync().Result;
                    var model = d.GetSemanticModelAsync().Result;
                    if (root == null || model == null) return Enumerable.Empty<IMethodSymbol>();

                    return root.DescendantNodes()
                        .OfType<MethodDeclarationSyntax>()
                        .Where(m => m.Modifiers.Any(SyntaxKind.AsyncKeyword) && !m.Identifier.Text.EndsWith("Async") && m.Identifier.Text != "Main")
                        .Select(m => model.GetDeclaredSymbol(m))
                        .Where(s => s != null)!;
                }).ToList();

            // 2️⃣ Collect all type symbols (class/interface) that need VM/DTO normalization
            var typeSymbols = solution.Projects
                .SelectMany(p => p.Documents)
                .SelectMany(d =>
                {
                    var root = d.GetSyntaxRootAsync().Result;
                    var model = d.GetSemanticModelAsync().Result;
                    if (root == null || model == null) return Enumerable.Empty<INamedTypeSymbol>();

                    return root.DescendantNodes()
                        .OfType<TypeDeclarationSyntax>()
                        .Select(t => model.GetDeclaredSymbol(t))
                        .Where(s => s != null && NeedsVmDtoNormalization(s.Name))!;
                }).ToList();

            // 3️⃣ Apply async method renames
            foreach (var symbol in asyncMethods)
            {
                string newName = symbol.Name + "Async";
                solution = await Renamer.RenameSymbolAsync(solution, symbol, newName, solution.Workspace.Options);
            }

            // 4️⃣ Apply VM/DTO type renames
            foreach (var symbol in typeSymbols)
            {
                solution = await RenameTypeWithReferencesAsync(solution, symbol);
            }

            // 5️⃣ Apply formatting (blank lines) using rewriter
            foreach (var project in solution.Projects)
            {
                foreach (var document in project.Documents)
                {
                    var root = await document.GetSyntaxRootAsync();
                    if (root == null) continue;

                    var rewriter = new VMDTOAndBlankLineRewriter();
                    var newRoot = rewriter.Visit(root);

                    if (newRoot != root)
                    {
                        solution = solution.WithDocumentSyntaxRoot(document.Id, newRoot);
                    }
                }
            }

            // 6️⃣ Apply all changes to workspace
            workspace.TryApplyChanges(solution);
        }

        /// <summary>
        /// Returns true if the type name ends with Vm, Vms, Dto, or Dtos.
        /// </summary>
        private static bool NeedsVmDtoNormalization(string name)
        {
            return name.EndsWith("Vm")
                || name.EndsWith("Vms")
                || name.EndsWith("Dto")
                || name.EndsWith("Dtos");
        }

        private static async Task<Solution> RenameTypeWithReferencesAsync(Solution solution, INamedTypeSymbol symbol)
        {
            string newName = NormalizeVmDto(symbol.Name);

            if (newName != symbol.Name)
            {
                solution = await Renamer.RenameSymbolAsync(solution, symbol, newName, solution.Workspace.Options);
            }

            return solution;
        }

        private static string NormalizeVmDto(string name)
        {
            if (name.EndsWith("Dtos"))
                return name.Substring(0, name.Length - 4) + "DTOs";
            if (name.EndsWith("Dto"))
                return name.Substring(0, name.Length - 3) + "DTO";
            if (name.EndsWith("Vms"))
                return name.Substring(0, name.Length - 3) + "VMs";
            if (name.EndsWith("Vm"))
                return name.Substring(0, name.Length - 2) + "VM";

            return name; // no change needed
        }
    }
}
