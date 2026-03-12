namespace CopilotEngineer.Core;

public sealed class EngineerCore(
    IIntentRouter intentRouter,
    IAgentRegistry agentRegistry,
    IWorkflowExecutor workflowExecutor,
    IContextProvider contextProvider) : IEngineerCore
{
    public async Task<EngineResponse> ProcessAsync(UserRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Input);

        var intent = intentRouter.Route(request);
        var context = await contextProvider.BuildAsync(request, intent, cancellationToken);

        if (intent.RequiresWorkflow && workflowExecutor.HasWorkflow(intent))
        {
            var workflowResult = await workflowExecutor.ExecuteAsync(intent, request, context, cancellationToken);
            return new EngineResponse(intent, workflowResult.Summary, workflowResult.WorkflowName, workflowResult.Steps);
        }

        var specialist = agentRegistry.Resolve(intent);
        var agentResult = await specialist.ExecuteAsync(request, context, cancellationToken);
        return new EngineResponse(intent, agentResult.Summary, agentResult.AgentName, agentResult.Recommendations);
    }
}
