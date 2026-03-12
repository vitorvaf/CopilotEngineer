using CopilotEngineer.Core;

namespace CopilotEngineer.Agents;

public static class SkillNames
{
    public const string AnalyzeStackTrace = "analyze_stack_trace";
    public const string AnalyzeSqlQuery = "analyze_sql_query";
    public const string ReviewCode = "review_code";
    public const string GenerateTests = "generate_tests";
}

public sealed class AnalyzeStackTraceSkill(ILLMService llmService) : ISkill
{
    public string Name => SkillNames.AnalyzeStackTrace;

    public Task<SkillExecutionResult> ExecuteAsync(UserRequest request, EngineerContext context, CancellationToken cancellationToken = default) =>
        llmService.ExecuteSkillAsync(
            Name,
            "DebugSpecialist",
            "Analise stack traces, sintomas e possiveis causas raiz. Priorize hipoteses verificaveis e proximos passos de investigacao.",
            request,
            context,
            cancellationToken);
}

public sealed class AnalyzeSqlQuerySkill(ILLMService llmService) : ISkill
{
    public string Name => SkillNames.AnalyzeSqlQuery;

    public Task<SkillExecutionResult> ExecuteAsync(UserRequest request, EngineerContext context, CancellationToken cancellationToken = default) =>
        llmService.ExecuteSkillAsync(
            Name,
            "DatabaseSpecialist",
            "Analise consultas SQL, joins, filtros e impactos de performance. Sugira otimizações, validacoes e pontos para EXPLAIN.",
            request,
            context,
            cancellationToken);
}

public sealed class ReviewCodeSkill(ILLMService llmService) : ISkill
{
    public string Name => SkillNames.ReviewCode;

    public Task<SkillExecutionResult> ExecuteAsync(UserRequest request, EngineerContext context, CancellationToken cancellationToken = default) =>
        llmService.ExecuteSkillAsync(
            Name,
            "CodeSpecialist",
            "Revise codigo com foco em corretude, riscos, aderencia as convencoes e oportunidades de refatoracao ou simplificacao.",
            request,
            context,
            cancellationToken);
}

public sealed class GenerateTestsSkill(ILLMService llmService) : ISkill
{
    public string Name => SkillNames.GenerateTests;

    public Task<SkillExecutionResult> ExecuteAsync(UserRequest request, EngineerContext context, CancellationToken cancellationToken = default) =>
        llmService.ExecuteSkillAsync(
            Name,
            "CodeSpecialist",
            "Gere recomendacoes de testes automatizados cobrindo fluxo principal, casos de borda, falhas esperadas e regressao.",
            request,
            context,
            cancellationToken);
}
