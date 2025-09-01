using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace Wheelzy.RefactorApp
{
    class Program    {
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

            solution = await CodeRefactorService.RefactorSolutionAsync(solution);

            workspace.TryApplyChanges(solution);
            Console.WriteLine("Refactor complete!");
        }
    }
}
