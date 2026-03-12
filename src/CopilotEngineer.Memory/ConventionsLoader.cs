namespace CopilotEngineer.Memory;

public sealed class ConventionsLoader(string filePath)
{
    public string FilePath { get; } = filePath;

    public async Task<ConventionsDefinition> LoadAsync(CancellationToken cancellationToken = default)
    {
        var lines = await File.ReadAllLinesAsync(FilePath, cancellationToken);

        var naming = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        List<string>? currentList = null;
        var patterns = new List<string>();

        foreach (var rawLine in lines)
        {
            var line = rawLine.TrimEnd();
            var trimmed = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            if (trimmed.Equals("conventions:", StringComparison.Ordinal) ||
                trimmed.Equals("naming:", StringComparison.Ordinal))
            {
                currentList = null;
                continue;
            }

            if (line.StartsWith("  patterns:", StringComparison.Ordinal))
            {
                currentList = patterns;
                continue;
            }

            if (line.StartsWith("    - ", StringComparison.Ordinal) && currentList is not null)
            {
                currentList.Add(line[6..].Trim());
                continue;
            }

            if (line.StartsWith("    ", StringComparison.Ordinal) && line.Contains(':'))
            {
                var separatorIndex = line.IndexOf(':', StringComparison.Ordinal);
                var key = line[4..separatorIndex].Trim();
                var value = line[(separatorIndex + 1)..].Trim();
                naming[key] = value;
                currentList = null;
            }
        }

        return new ConventionsDefinition(naming, patterns);
    }
}
