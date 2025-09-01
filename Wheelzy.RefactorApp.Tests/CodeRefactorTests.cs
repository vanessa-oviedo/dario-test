using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Wheelzy.RefactorApp.Tests
{
    [TestClass]
    public class CodeRefactorTests    {
        private Solution CreateSolutionWithCode(string code, string docName = "Test.cs")
        {
            var workspace = new AdhocWorkspace();
            var project = workspace.AddProject("TestProject", LanguageNames.CSharp);
            project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            var document = project.AddDocument(docName, code);
            return document.Project.Solution;
        }

        [TestMethod]
        public async Task AsyncMethod_ShouldBeRenamed_ExceptMainAsync()
        {
            //Arrange
            var code = @"
            public class TestClass
            {
                public async Task DoWork() {}
                public async Task Main() {}
            }";
            var solution = CreateSolutionWithCode(code);

            //Act

            solution = await CodeRefactorService.RefactorSolutionAsync(solution);

            var doc = solution.Projects.First().Documents.First();
            var text = (await doc.GetTextAsync()).ToString();

            //Assert

            text.Should().Contain("DoWorkAsync");
            text.Should().Contain("Main");        // Main no debe cambiar
            text.Should().NotContain("DoWork()"); // Original desaparece
        }



        [TestMethod]
        public async Task ClassDto_ShouldBeRenamed_ToDTOAsync()
        {
            //Arrange
            var code = @"
                public class CustomerDto { }";
            var solution = CreateSolutionWithCode(code);

            //Act
            solution = await CodeRefactorService.RefactorSolutionAsync(solution);

            var doc = solution.Projects.First().Documents.First();
            var text = (await doc.GetTextAsync()).ToString();

            //Assert
            text.Should().Contain("CustomerDTO");
            text.Should().NotContain("CustomerDto");
        }



        [TestMethod]
        public async Task ClassVm_ShouldBeRenamed_ToVMAsync()
        {
            //Arrange
            var code = @"
public class OrderVm { }";
            var solution = CreateSolutionWithCode(code);

            //Act
            solution = await CodeRefactorService.RefactorSolutionAsync(solution);

            var doc = solution.Projects.First().Documents.First();
            var text = (await doc.GetTextAsync()).ToString();

            //Assert
            text.Should().Contain("OrderVM");
            text.Should().NotContain("OrderVm");
        }



        [TestMethod]
        public async Task BlankLines_ShouldBeAdded_BetweenMembersAsync()
        {
            //Arrange
            var code = @"
public class TestClass
{
    public void Method1() {}
    public void Method2() {}
}";
            var solution = CreateSolutionWithCode(code);

            //Act
            solution = await CodeRefactorService.RefactorSolutionAsync(solution);

            var doc = solution.Projects.First().Documents.First();
            var root = await doc.GetSyntaxRootAsync();
            var classNode = root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
            var members = classNode.Members;

            //Assert
            for (int i = 0; i < members.Count - 1; i++)
            {
                var trailing = members[i].GetTrailingTrivia().ToString();
                // Check that there are at least 2 newlines
                trailing.Should().Contain("\r\n\r\n");
            }
        }
    }
}