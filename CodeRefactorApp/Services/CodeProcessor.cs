namespace CodeRefactorApp.Services
{
    using System.Text.RegularExpressions;

    namespace CodeRefactorApp
    {
        public static class CodeProcessor
        {
            public static string Process(string code)
            {
                string result = code;

                // (a) Add Async suffix to async methods without it
                result = Regex.Replace(result,
                    @"async\s+Task(?:<[^>]+>)?\s+([A-Za-z0-9_]+)\s*\(",
                    match =>
                    {
                        string methodName = match.Groups[1].Value;
                        if (!methodName.EndsWith("Async"))
                        {
                            return match.Value.Replace(methodName, methodName + "Async");
                        }
                        return match.Value;
                    });

                // (b) Normalize suffixes Vm/Vms/Dto/Dtos
                result = Regex.Replace(result, @"\b(\w*?)(Vm|Vms|Dto|Dtos)\b", m =>
                {
                    return m.Groups[1].Value + m.Groups[2].Value
                        .Replace("Vm", "VM")
                        .Replace("Vms", "VMs")
                        .Replace("Dto", "DTO")
                        .Replace("Dtos", "DTOs");
                });

                // (c) Ensure blank line between methods
                result = Regex.Replace(result,
                    @"\}\s*\n\s*(public|private|protected|internal)\s",
                    "}\n\n $1 ");

                return result;
            }
        }
    }

}
