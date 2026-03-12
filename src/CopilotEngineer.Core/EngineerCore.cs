namespace CopilotEngineer.Core;

public sealed class EngineerCore(
    IIntentRouter intentRouter,
    IAgentRegistry agentRegistry,
    IWorkflowExecutor workflowExecutor,
    IContextProvider contextProvider)
{
    public async Task<EngineerResponse> ProcessAsync(EngineerRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var intent = intentRouter.Route(request.Input);
        var context = await contextProvider.BuildAsync(request, intent, cancellationToken);

        if (intent.RequiresWorkflow && workflowExecutor.HasWorkflow(intent))
        {
            var workflowResult = await workflowExecutor.ExecuteAsync(intent, request, context, cancellationToken);
            return new EngineerResponse(intent, workflowResult.Summary, workflowResult.WorkflowName, workflowResult.Steps);
        }

        var agent = agentRegistry.Resolve(intent);
        var agentResult = await agent.ExecuteAsync(request, context, cancellationToken);
        return new EngineerResponse(intent, agentResult.Summary, agentResult.AgentName, agentResult.Recommendations);
    }
}
