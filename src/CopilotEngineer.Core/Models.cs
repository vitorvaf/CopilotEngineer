namespace CopilotEngineer.Core;

public sealed record EngineerRequest(string Input);

public sealed record EngineerContext(
    string ProjectName,
    string ArchitectureSummary,
    IReadOnlyCollection<string> AvailableWorkflows,
    IReadOnlyCollection<string> AvailableTools);

public sealed record EngineeringIntent(string Type, bool RequiresWorkflow)
{
    public static readonly EngineeringIntent Ask = new("ask", false);
    public static readonly EngineeringIntent Debug = new("debug", true);
    public static readonly EngineeringIntent Sql = new("sql", true);
    public static readonly EngineeringIntent Review = new("review", false);
}

public sealed record AgentExecutionResult(string AgentName, string Summary, IReadOnlyCollection<string> Recommendations);

public sealed record WorkflowExecutionResult(string WorkflowName, string Summary, IReadOnlyCollection<string> Steps);

public sealed record EngineerResponse(
    EngineeringIntent Intent,
    string Summary,
    string Source,
    IReadOnlyCollection<string> Recommendations);
