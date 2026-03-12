namespace CopilotEngineer.Memory;

public static class MemoryPaths
{
    public static string ResolveRepositoryRoot()
    {
        var currentDirectory = new DirectoryInfo(AppContext.BaseDirectory);

        while (currentDirectory is not null)
        {
            var solutionFile = Path.Combine(currentDirectory.FullName, "CopilotEngineer.sln");

            if (File.Exists(solutionFile))
            {
                return currentDirectory.FullName;
            }

            currentDirectory = currentDirectory.Parent;
        }

        return Directory.GetCurrentDirectory();
    }
}
