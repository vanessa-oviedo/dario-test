using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis;
using Wheelzy.RefactorApp.Services;

public static class CodeRefactorService{
    /// <summary>
    /// Renames async methods and VM/DTO types and formats blank lines in a solution.
    /// </summary>
    public static async Task<Solution> RefactorSolutionAsync(Solution solution)
    {
        // 1️⃣ Collect all async method symbols (excluding Main)
        var asyncMethods = solution.Projects
            .SelectMany(p => p.Documents)
            .SelectMany(d =>
            {
                var root = d.GetSyntaxRootAsync().Result;
                var model = d.GetSemanticModelAsync().Result;
                if (root == null || model == null) return Enumerable.Empty<IMethodSymbol>();

                return root.DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Where(m =>
                        m.Modifiers.Any(SyntaxKind.AsyncKeyword) &&
                        !m.Identifier.Text.EndsWith("Async") &&
                        m.Identifier.Text != "Main")
                    .Select(m => model.GetDeclaredSymbol(m))
                    .Where(s => s != null)!;
            }).ToList();

        // 2️⃣ Collect all type symbols that need VM/DTO normalization
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

        return solution;
    }



    public static bool NeedsVmDtoNormalization(string name)
        => name.EndsWith("Vm") || name.EndsWith("Vms") || name.EndsWith("Dto") || name.EndsWith("Dtos");



    public static async Task<Solution> RenameTypeWithReferencesAsync(Solution solution, INamedTypeSymbol symbol)
    {
        string newName = NormalizeVmDto(symbol.Name);

        if (newName != symbol.Name)
        {
            solution = await Renamer.RenameSymbolAsync(solution, symbol, newName, solution.Workspace.Options);
        }

        return solution;
    }



    public static string NormalizeVmDto(string name)
    {
        if (name.EndsWith("Dtos"))
            return name.Substring(0, name.Length - 4) + "DTOs";
        if (name.EndsWith("Dto"))
            return name.Substring(0, name.Length - 3) + "DTO";
        if (name.EndsWith("Vms"))
            return name.Substring(0, name.Length - 3) + "VMs";
        if (name.EndsWith("Vm"))
            return name.Substring(0, name.Length - 2) + "VM";

        return name;
    }
}
