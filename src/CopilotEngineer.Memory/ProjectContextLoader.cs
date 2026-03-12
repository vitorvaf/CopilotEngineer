namespace CopilotEngineer.Memory;

public sealed class ProjectContextLoader(string filePath)
{
    public string FilePath { get; } = filePath;

    public async Task<ProjectContextDefinition> LoadAsync(CancellationToken cancellationToken = default)
    {
        var lines = await File.ReadAllLinesAsync(FilePath, cancellationToken);

        string? projectName = null;
        string? architectureSummary = null;
        List<string>? currentList = null;
        var stack = new List<string>();
        var workflows = new List<string>();
        var tools = new List<string>();

        foreach (var rawLine in lines)
        {
            var line = rawLine.TrimEnd();
            var trimmed = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            if (trimmed.Equals("project:", StringComparison.Ordinal))
            {
                currentList = null;
                continue;
            }

            if (line.StartsWith("  name:", StringComparison.Ordinal))
            {
                projectName = ReadScalarValue(line);
                currentList = null;
                continue;
            }

            if (line.StartsWith("  architecture:", StringComparison.Ordinal))
            {
                architectureSummary = ReadScalarValue(line);
                currentList = null;
                continue;
            }

            if (line.StartsWith("  stack:", StringComparison.Ordinal))
            {
                currentList = stack;
                continue;
            }

            if (line.StartsWith("  workflows:", StringComparison.Ordinal))
            {
                currentList = workflows;
                continue;
            }

            if (line.StartsWith("  tools:", StringComparison.Ordinal))
            {
                currentList = tools;
                continue;
            }

            if (line.StartsWith("    - ", StringComparison.Ordinal) && currentList is not null)
            {
                currentList.Add(line[6..].Trim());
                continue;
            }

            currentList = null;
        }

        return new ProjectContextDefinition(
            projectName ?? throw new InvalidOperationException("project.name nao configurado em project-context.yaml."),
            architectureSummary ?? throw new InvalidOperationException("project.architecture nao configurado em project-context.yaml."),
            stack,
            workflows,
            tools);
    }

    private static string ReadScalarValue(string line)
    {
        var separatorIndex = line.IndexOf(':', StringComparison.Ordinal);

        return separatorIndex >= 0
            ? line[(separatorIndex + 1)..].Trim()
            : string.Empty;
    }
}
