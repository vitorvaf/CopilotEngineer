namespace CopilotEngineer.Core;

public interface IIntentRouter
{
    EngineeringIntent Route(string request);
}

public interface IEngineerAgent
{
    string Name { get; }

    bool CanHandle(EngineeringIntent intent);

    Task<AgentExecutionResult> ExecuteAsync(EngineerRequest request, EngineerContext context, CancellationToken cancellationToken = default);
}

public interface IAgentRegistry
{
    IEngineerAgent Resolve(EngineeringIntent intent);

    IReadOnlyCollection<IEngineerAgent> List();
}

public interface IWorkflowExecutor
{
    bool HasWorkflow(EngineeringIntent intent);

    Task<WorkflowExecutionResult> ExecuteAsync(EngineeringIntent intent, EngineerRequest request, EngineerContext context, CancellationToken cancellationToken = default);
}

public interface IContextProvider
{
    Task<EngineerContext> BuildAsync(EngineerRequest request, EngineeringIntent intent, CancellationToken cancellationToken = default);
}
