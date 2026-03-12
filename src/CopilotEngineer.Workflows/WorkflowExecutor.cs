using CopilotEngineer.Core;

namespace CopilotEngineer.Workflows;

public sealed class WorkflowExecutor : IWorkflowExecutor
{
    private static readonly Dictionary<string, string> WorkflowNames = new(StringComparer.OrdinalIgnoreCase)
    {
        [EngineeringIntent.Debug.Type] = "bug-investigation",
        [EngineeringIntent.Sql.Type] = "sql-analysis"
    };

    public bool HasWorkflow(EngineeringIntent intent) => WorkflowNames.ContainsKey(intent.Type);

    public Task<WorkflowExecutionResult> ExecuteAsync(EngineeringIntent intent, EngineerRequest request, EngineerContext context, CancellationToken cancellationToken = default)
    {
        if (!WorkflowNames.TryGetValue(intent.Type, out var workflowName))
        {
            throw new InvalidOperationException($"Workflow nao configurado para '{intent.Type}'.");
        }

        var result = new WorkflowExecutionResult(
            workflowName,
            $"Workflow '{workflowName}' preparado para '{request.Input}'.",
            ["Montar contexto relevante.", "Executar especialista principal.", "Consolidar recomendacoes."]);

        return Task.FromResult(result);
    }
}
