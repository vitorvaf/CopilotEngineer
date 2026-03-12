using CopilotEngineer.Core;

namespace CopilotEngineer.Workflows;

public sealed class WorkflowExecutor : IWorkflowExecutor
{
    private static readonly IReadOnlyDictionary<string, string> WorkflowNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        [Intent.Issue.Name] = "issue-analysis",
        [Intent.Debug.Name] = "bug-investigation",
        [Intent.Sql.Name] = "sql-analysis"
    };

    private readonly IAgentRegistry agentRegistry;
    private readonly WorkflowCatalog workflowCatalog;

    public WorkflowExecutor(IAgentRegistry agentRegistry)
        : this(agentRegistry, ResolveWorkflowsDirectory())
    {
    }

    public WorkflowExecutor(IAgentRegistry agentRegistry, string workflowsDirectory)
    {
        ArgumentNullException.ThrowIfNull(agentRegistry);
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowsDirectory);

        this.agentRegistry = agentRegistry;
        workflowCatalog = new WorkflowCatalog(workflowsDirectory);
    }

    public bool HasWorkflow(Intent intent) => WorkflowNames.ContainsKey(intent.Name);

    public async Task<WorkflowExecutionResult> ExecuteAsync(Intent intent, UserRequest request, EngineerContext context, CancellationToken cancellationToken = default)
    {
        if (!WorkflowNames.TryGetValue(intent.Name, out var workflowName))
        {
            throw new InvalidOperationException($"Workflow nao configurado para '{intent.Name}'.");
        }

        var definition = workflowCatalog.Get(workflowName);
        var executionSummaries = new List<string>();
        var steps = new List<string>();

        foreach (var step in definition.Steps)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var specialist = agentRegistry.Resolve(new Intent(step.Name, step.SpecialistName, false));
            var stepRequest = BuildStepRequest(request, definition.Name, step);
            var agentResult = await specialist.ExecuteAsync(stepRequest, context, cancellationToken);

            executionSummaries.Add($"{step.Name}: {agentResult.Summary}");
            steps.Add($"{step.Name} [{agentResult.AgentName}]");

            foreach (var recommendation in agentResult.Recommendations)
            {
                steps.Add($"- {recommendation}");
            }
        }

        return new WorkflowExecutionResult(
            definition.Name,
            string.Join(Environment.NewLine, executionSummaries),
            steps);
    }

    private static UserRequest BuildStepRequest(UserRequest request, string workflowName, WorkflowStepDefinition step)
    {
        Dictionary<string, string>? metadata = null;
        if (request.Metadata is not null)
        {
            metadata = new Dictionary<string, string>(request.Metadata, StringComparer.OrdinalIgnoreCase);
        }

        metadata ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        metadata["workflow"] = workflowName;
        metadata["workflow_step"] = step.Name;
        metadata["workflow_instruction"] = step.Instruction;

        var input = $"{request.Input} | workflow:{workflowName} | etapa:{step.Name} | objetivo:{step.Instruction}";
        return new UserRequest(input, metadata);
    }

    private static string ResolveWorkflowsDirectory()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            var candidate = Path.Combine(current.FullName, "workflows");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("Diretorio de workflows nao encontrado a partir do diretorio base da aplicacao.");
    }
}
