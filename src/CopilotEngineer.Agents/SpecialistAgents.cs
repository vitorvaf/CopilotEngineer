using CopilotEngineer.Core;

namespace CopilotEngineer.Agents;

public sealed class DebugSpecialistAgent : IEngineerSpecialist
{
    public string Name => "DebugSpecialist";

    public Task<AgentExecutionResult> ExecuteAsync(UserRequest request, EngineerContext context, CancellationToken cancellationToken = default)
    {
        var result = new AgentExecutionResult(
            Name,
            $"Analisando possivel falha reportada em '{request.Input}'.",
            ["Correlacionar stack trace com logs.", "Verificar dependencias externas e configuracoes locais."]);

        return Task.FromResult(result);
    }
}

public sealed class DatabaseSpecialistAgent : IEngineerSpecialist
{
    public string Name => "DatabaseSpecialist";

    public Task<AgentExecutionResult> ExecuteAsync(UserRequest request, EngineerContext context, CancellationToken cancellationToken = default)
    {
        var result = new AgentExecutionResult(
            Name,
            $"Analisando consulta SQL em '{request.Input}'.",
            ["Mapear joins criticos.", "Revisar indices e plano de execucao."]);

        return Task.FromResult(result);
    }
}

public sealed class CodeSpecialistAgent : IEngineerSpecialist
{
    public string Name => "CodeSpecialist";

    public Task<AgentExecutionResult> ExecuteAsync(UserRequest request, EngineerContext context, CancellationToken cancellationToken = default)
    {
        var result = new AgentExecutionResult(
            Name,
            $"Avaliando solicitacao '{request.Input}' com base nas convencoes do projeto.",
            ["Identificar smells de codigo.", "Sugerir proximos testes automatizados."]);

        return Task.FromResult(result);
    }
}
