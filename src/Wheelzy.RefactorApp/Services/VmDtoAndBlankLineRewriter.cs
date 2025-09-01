using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Wheelzy.RefactorApp.Services
{
    /// <summary>
    /// Rewriter to normalize VM/DTO suffixes and insert blank lines between methods/interfaces.
    /// </summary>
    public class VMDTOAndBlankLineRewriter: CSharpSyntaxRewriter
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



        private SyntaxList<MemberDeclarationSyntax> InsertBlankLines(SyntaxList<MemberDeclarationSyntax> members)
        {
            var newMembers = new SyntaxList<MemberDeclarationSyntax>();

            for (int i = 0; i < members.Count; i++)
            {
                var member = members[i];

                // Only add extra blank lines if it's not the last member
                if (i < members.Count - 1)
                {
                    var trailingTrivia = member.GetTrailingTrivia()
                        .Add(SyntaxFactory.CarriageReturnLineFeed)
                        .Add(SyntaxFactory.CarriageReturnLineFeed);

                    member = member.WithTrailingTrivia(trailingTrivia);
                }

                newMembers = newMembers.Add(member);
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
