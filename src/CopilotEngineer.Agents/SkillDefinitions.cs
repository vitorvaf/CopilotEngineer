using CopilotEngineer.Core;

namespace CopilotEngineer.Agents;

public static class SkillNames
{
    public const string AnalyzeStackTrace = "analyze_stack_trace";
    public const string AnalyzeSqlQuery = "analyze_sql_query";
    public const string ReviewCode = "review_code";
    public const string GenerateTests = "generate_tests";
}

public sealed class AnalyzeStackTraceSkill : ISkill
{
    public string Name => SkillNames.AnalyzeStackTrace;

    public Task<SkillExecutionResult> ExecuteAsync(UserRequest request, EngineerContext context, CancellationToken cancellationToken = default)
    {
        var result = new SkillExecutionResult(
            Name,
            $"Placeholder: analisando stack trace descrita em '{request.Input}'.",
            ["Extrair excecao principal.", "Mapear frames relevantes para investigacao."]);

        return Task.FromResult(result);
    }
}

public sealed class AnalyzeSqlQuerySkill : ISkill
{
    public string Name => SkillNames.AnalyzeSqlQuery;

    public Task<SkillExecutionResult> ExecuteAsync(UserRequest request, EngineerContext context, CancellationToken cancellationToken = default)
    {
        var result = new SkillExecutionResult(
            Name,
            $"Placeholder: analisando consulta SQL mencionada em '{request.Input}'.",
            ["Identificar joins mais custosos.", "Sugerir verificacao de indices e filtros."]);

        return Task.FromResult(result);
    }
}

public sealed class ReviewCodeSkill : ISkill
{
    public string Name => SkillNames.ReviewCode;

    public Task<SkillExecutionResult> ExecuteAsync(UserRequest request, EngineerContext context, CancellationToken cancellationToken = default)
    {
        var result = new SkillExecutionResult(
            Name,
            $"Placeholder: revisando codigo relacionado a '{request.Input}'.",
            ["Avaliar aderencia as convencoes.", "Apontar pontos de refatoracao imediata."]);

        return Task.FromResult(result);
    }
}

public sealed class GenerateTestsSkill : ISkill
{
    public string Name => SkillNames.GenerateTests;

    public Task<SkillExecutionResult> ExecuteAsync(UserRequest request, EngineerContext context, CancellationToken cancellationToken = default)
    {
        var result = new SkillExecutionResult(
            Name,
            $"Placeholder: gerando ideias de testes para '{request.Input}'.",
            ["Cobrir fluxo principal.", "Adicionar cenario de falha e validacao de borda."]);

        return Task.FromResult(result);
    }
}
