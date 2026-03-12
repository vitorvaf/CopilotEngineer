using CopilotEngineer.Core;

namespace CopilotEngineer.Workflows;

internal sealed class WorkflowCatalog
{
    private readonly string workflowsDirectory;
    private readonly Dictionary<string, WorkflowDefinition> definitions = new(StringComparer.OrdinalIgnoreCase);

    private static readonly IReadOnlyDictionary<string, WorkflowDefinition> FallbackDefinitions =
        new Dictionary<string, WorkflowDefinition>(StringComparer.OrdinalIgnoreCase)
        {
            ["issue-analysis"] = new WorkflowDefinition(
                "issue-analysis",
                [
                    new WorkflowStepDefinition("classify", "CodeSpecialist", "Classificar a solicitacao e delimitar escopo."),
                    new WorkflowStepDefinition("gather-context", "DebugSpecialist", "Levantar contexto tecnico e riscos iniciais."),
                    new WorkflowStepDefinition("propose-next-actions", "CodeSpecialist", "Consolidar proximos passos objetivos.")
                ]),
            ["bug-investigation"] = new WorkflowDefinition(
                "bug-investigation",
                [
                    new WorkflowStepDefinition("inspect-stack-trace", "DebugSpecialist", "Inspecionar stack trace e sintomas principais."),
                    new WorkflowStepDefinition("correlate-logs", "DatabaseSpecialist", "Correlacionar dependencias e possiveis gargalos de dados."),
                    new WorkflowStepDefinition("suggest-fix", "CodeSpecialist", "Sugerir direcao de correcao e validacoes.")
                ]),
            ["sql-analysis"] = new WorkflowDefinition(
                "sql-analysis",
                [
                    new WorkflowStepDefinition("parse-query", "DatabaseSpecialist", "Analisar a consulta e seus joins/filtros."),
                    new WorkflowStepDefinition("inspect-plan", "DebugSpecialist", "Avaliar sintomas e impacto operacional."),
                    new WorkflowStepDefinition("suggest-indexes", "CodeSpecialist", "Consolidar a proposta de otimizacao.")
                ])
        };

    public WorkflowCatalog(string workflowsDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowsDirectory);
        this.workflowsDirectory = workflowsDirectory;
    }

    public WorkflowDefinition Get(string workflowName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowName);

        if (definitions.TryGetValue(workflowName, out var cached))
        {
            return cached;
        }

        var definition = LoadDefinition(workflowName);
        definitions[workflowName] = definition;
        return definition;
    }

    private WorkflowDefinition LoadDefinition(string workflowName)
    {
        var filePath = Path.Combine(workflowsDirectory, $"{workflowName}.yaml");
        if (!File.Exists(filePath))
        {
            if (FallbackDefinitions.TryGetValue(workflowName, out var fallbackDefinition))
            {
                return fallbackDefinition;
            }

            throw new InvalidOperationException($"Arquivo de workflow nao encontrado: '{filePath}'.");
        }

        var lines = File.ReadAllLines(filePath);
        string? name = null;
        var stepNames = new List<string>();

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();
            if (line.StartsWith("name:", StringComparison.OrdinalIgnoreCase))
            {
                name = line["name:".Length..].Trim();
                continue;
            }

            if (line.StartsWith("-", StringComparison.Ordinal))
            {
                stepNames.Add(line[1..].Trim());
            }
        }

        name ??= workflowName;

        if (!FallbackDefinitions.TryGetValue(name, out var fallback))
        {
            throw new InvalidOperationException($"Workflow '{name}' nao possui mapeamento de etapas para especialistas.");
        }

        if (stepNames.Count == 0)
        {
            return fallback;
        }

        var mappedSteps = stepNames
            .Select(stepName => fallback.Steps.FirstOrDefault(step => string.Equals(step.Name, stepName, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException($"Etapa '{stepName}' nao configurada para o workflow '{name}'."))
            .ToArray();

        return new WorkflowDefinition(name, mappedSteps);
    }
}
