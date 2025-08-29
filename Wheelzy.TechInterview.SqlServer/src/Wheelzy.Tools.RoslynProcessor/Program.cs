using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using System.Text.RegularExpressions;

if (args.Length == 0)
{
    Console.Error.WriteLine("Usage: roslynproc <folder>");
    return 1;
}

var rootFolder = args[0];
var files = Directory.EnumerateFiles(rootFolder, "*.cs", SearchOption.AllDirectories).ToList();
Console.WriteLine($"Procesando {files.Count} archivos...");
foreach (var file in files)
{
    var text = await File.ReadAllTextAsync(file);
    var tree = CSharpSyntaxTree.ParseText(text);
    var root = await tree.GetRootAsync();

    var rewriter = new ProcessorRewriter();
    var newRoot = (CompilationUnitSyntax)rewriter.Visit(root);

    var newText = newRoot.NormalizeWhitespace(elasticTrivia: true).ToFullString();
    newText = Regex.Replace(newText, @"\n\s*\}\r?\n\s*(?=\w|\[)", m => "\n}\n\n", RegexOptions.Multiline);

    if (!string.Equals(text, newText, StringComparison.Ordinal))
    {
        await File.WriteAllTextAsync(file, newText, Encoding.UTF8);
        Console.WriteLine($"✓ {file}");
    }
}
return 0;

class ProcessorRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        var hasAsync = node.Modifiers.Any(m => m.IsKind(SyntaxKind.AsyncKeyword));
        if (hasAsync && !node.Identifier.Text.EndsWith("Async", StringComparison.Ordinal))
        {
            node = node.WithIdentifier(SyntaxFactory.Identifier(node.Identifier.Text + "Async"));
        }
        return base.VisitMethodDeclaration(node);
    }

    public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
    {
        var id = node.Identifier.Text;
        string Replace(string s) =>
            s.EndsWith("Vm") ? s[..^2] + "VM" :
            s.EndsWith("Vms") ? s[..^3] + "VMs" :
            s.EndsWith("Dto") ? s[..^3] + "DTO" :
            s.EndsWith("Dtos") ? s[..^4] + "DTOs" :
            s;

        var rep = Replace(id);
        if (!ReferenceEquals(rep, id))
            return node.WithIdentifier(SyntaxFactory.Identifier(rep));

        return base.VisitIdentifierName(node);
    }
}
