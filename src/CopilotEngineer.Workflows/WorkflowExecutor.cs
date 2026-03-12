using CopilotEngineer.Core;

namespace CopilotEngineer.Workflows;

public sealed class WorkflowExecutor : IWorkflowExecutor
{
    private static readonly Dictionary<string, string> WorkflowNames = new(StringComparer.OrdinalIgnoreCase)
    {
        [Intent.Debug.Name] = "bug-investigation",
        [Intent.Sql.Name] = "sql-analysis"
    };

    public bool HasWorkflow(Intent intent) => WorkflowNames.ContainsKey(intent.Name);

    public Task<WorkflowExecutionResult> ExecuteAsync(Intent intent, UserRequest request, EngineerContext context, CancellationToken cancellationToken = default)
    {
        if (!WorkflowNames.TryGetValue(intent.Name, out var workflowName))
        {
            throw new InvalidOperationException($"Workflow nao configurado para '{intent.Name}'.");
        }

        var result = new WorkflowExecutionResult(
            workflowName,
            $"Workflow '{workflowName}' preparado para '{request.Input}'.",
            ["Montar contexto relevante.", "Executar especialista principal.", "Consolidar recomendacoes."]);

        return Task.FromResult(result);
    }
}
