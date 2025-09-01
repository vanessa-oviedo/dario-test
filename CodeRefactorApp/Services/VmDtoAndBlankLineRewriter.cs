using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeRefactorTool
{
    /// <summary>
    /// Rewriter to normalize VM/DTO suffixes and insert blank lines between methods/interfaces.
    /// </summary>
    class VmDtoAndBlankLineRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            // Normalize VM/DTO suffix in class name
            var newName = NormalizeVmDto(node.Identifier.Text);
            node = node.WithIdentifier(SyntaxFactory.Identifier(newName));

            // Insert blank lines between members
            node = node.WithMembers(InsertBlankLines(node.Members));

            return base.VisitClassDeclaration(node);
        }

        public override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            // Normalize VM/DTO suffix in interface name
            var newName = NormalizeVmDto(node.Identifier.Text);
            node = node.WithIdentifier(SyntaxFactory.Identifier(newName));

            // Insert blank lines between members
            node = node.WithMembers(InsertBlankLines(node.Members));

            return base.VisitInterfaceDeclaration(node);
        }

        private SyntaxList<MemberDeclarationSyntax> InsertBlankLines(SyntaxList<MemberDeclarationSyntax> members)
        {
            var newMembers = new SyntaxList<MemberDeclarationSyntax>();

            for (int i = 0; i < members.Count; i++)
            {
                var member = members[i];
                newMembers = newMembers.Add(member);

                if (i < members.Count - 1)
                {
                    var trivia = member.GetTrailingTrivia().Add(SyntaxFactory.CarriageReturnLineFeed).Add(SyntaxFactory.CarriageReturnLineFeed);
                    newMembers = newMembers.Replace(member, member.WithTrailingTrivia(trivia));
                }
            }

            return newMembers;
        }

        private string NormalizeVmDto(string name)
        {
            return name
                .Replace("Vm", "VM")
                .Replace("Vms", "VMs")
                .Replace("Dto", "DTO")
                .Replace("Dtos", "DTOs");
        }
    }
}
