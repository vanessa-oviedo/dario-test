using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis;

class AsyncVMDTORewriter: CSharpSyntaxRewriter
{
    private readonly SemanticModel _model;
    private readonly Solution _solution;

    public AsyncVMDTORewriter(SemanticModel model, Solution solution)
    {
        _model = model;
        _solution = solution;
    }

    public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        if (node.Modifiers.Any(SyntaxKind.AsyncKeyword) && !node.Identifier.Text.EndsWith("Async"))
        {
            var symbol = _model.GetDeclaredSymbol(node);
            if (symbol != null)
            {
                string newName = node.Identifier.Text + "Async";
                // Use _solution instead of _model.Compilation.Solution
                Renamer.RenameSymbolAsync(_solution, symbol, newName, _solution.Workspace.Options).Wait();
                node = node.WithIdentifier(SyntaxFactory.Identifier(newName));
            }
        }

        return base.VisitMethodDeclaration(node);
    }
}
