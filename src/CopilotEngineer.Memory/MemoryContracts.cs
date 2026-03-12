namespace CopilotEngineer.Memory;

public sealed record ProjectContextDefinition(
    string ProjectName,
    string ArchitectureSummary,
    IReadOnlyCollection<string> Stack,
    IReadOnlyCollection<string> Workflows,
    IReadOnlyCollection<string> Tools);

public sealed record ConventionsDefinition(
    IReadOnlyDictionary<string, string> Naming,
    IReadOnlyCollection<string> Patterns);
