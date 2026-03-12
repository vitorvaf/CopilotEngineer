namespace CopilotEngineer.Core;

public sealed record UserRequest(
    string Input,
    IReadOnlyDictionary<string, string>? Metadata = null);

public sealed record BugMemoryRecord(
    long Id,
    string Title,
    string Summary,
    string RootCause,
    string Resolution,
    IReadOnlyCollection<string> Tags,
    DateTimeOffset CreatedAtUtc);

public sealed record EngineerContext(
    string ProjectName,
    string ArchitectureSummary,
    IReadOnlyCollection<string> AvailableWorkflows,
    IReadOnlyCollection<string> AvailableTools,
    IReadOnlyCollection<string> ProjectStack,
    IReadOnlyDictionary<string, string> NamingConventions,
    IReadOnlyCollection<string> DesignPatterns,
    IReadOnlyCollection<BugMemoryRecord> RelatedBugMemories);

public sealed record Intent(
    string Name,
    string Specialist,
    bool RequiresWorkflow)
{
    public static readonly Intent Ask = new("ask", "CodeSpecialist", false);
    public static readonly Intent Debug = new("debug", "DebugSpecialist", true);
    public static readonly Intent Sql = new("sql", "DatabaseSpecialist", true);
    public static readonly Intent Review = new("review", "CodeSpecialist", false);
}

public sealed record AgentExecutionResult(
    string AgentName,
    string Summary,
    IReadOnlyCollection<string> Recommendations);

public sealed record SkillExecutionResult(
    string SkillName,
    string Summary,
    IReadOnlyCollection<string> Outputs);

public sealed record WorkflowExecutionResult(
    string WorkflowName,
    string Summary,
    IReadOnlyCollection<string> Steps);

public sealed record EngineResponse(
    Intent Intent,
    string Summary,
    string Source,
    IReadOnlyCollection<string> Recommendations);
