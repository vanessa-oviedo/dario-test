using CodeRefactorApp.Services.CodeRefactorApp;

namespace CodeRefactorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter folder path:");
            string folder = Console.ReadLine() ?? string.Empty;

            if (!Directory.Exists(folder))
            {
                Console.WriteLine("Folder not found.");
                return;
            }

            var files = Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                string code = File.ReadAllText(file);
                string updated = CodeProcessor.Process(code);

                if (updated != code)
                {
                    File.WriteAllText(file, updated);
                    Console.WriteLine($"Updated: {file}");
                }
            }

            Console.WriteLine("Processing completed.");
        }
    }
}