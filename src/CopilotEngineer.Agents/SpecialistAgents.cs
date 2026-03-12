using CopilotEngineer.Core;

namespace CopilotEngineer.Agents;

public sealed class DebugSpecialistAgent(ISkillRegistry skillRegistry) : IEngineerSpecialist
{
    public string Name => "DebugSpecialist";

    public async Task<AgentExecutionResult> ExecuteAsync(UserRequest request, EngineerContext context, CancellationToken cancellationToken = default)
    {
        var skillResult = await skillRegistry.Resolve(SkillNames.AnalyzeStackTrace)
            .ExecuteAsync(request, context, cancellationToken);

        return new AgentExecutionResult(
            Name,
            skillResult.Summary,
            skillResult.Outputs);
    }
}

public sealed class DatabaseSpecialistAgent(ISkillRegistry skillRegistry) : IEngineerSpecialist
{
    public string Name => "DatabaseSpecialist";

    public async Task<AgentExecutionResult> ExecuteAsync(UserRequest request, EngineerContext context, CancellationToken cancellationToken = default)
    {
        var skillResult = await skillRegistry.Resolve(SkillNames.AnalyzeSqlQuery)
            .ExecuteAsync(request, context, cancellationToken);

        return new AgentExecutionResult(
            Name,
            skillResult.Summary,
            skillResult.Outputs);
    }
}

public sealed class CodeSpecialistAgent(ISkillRegistry skillRegistry) : IEngineerSpecialist
{
    public string Name => "CodeSpecialist";

    public async Task<AgentExecutionResult> ExecuteAsync(UserRequest request, EngineerContext context, CancellationToken cancellationToken = default)
    {
        var primarySkillName = IsTestGenerationRequest(request.Input)
            ? SkillNames.GenerateTests
            : SkillNames.ReviewCode;

        var skillResult = await skillRegistry.Resolve(primarySkillName)
            .ExecuteAsync(request, context, cancellationToken);

        return new AgentExecutionResult(
            Name,
            skillResult.Summary,
            skillResult.Outputs);
    }

    private static bool IsTestGenerationRequest(string input) =>
        input.Contains("test", StringComparison.OrdinalIgnoreCase) ||
        input.Contains("teste", StringComparison.OrdinalIgnoreCase);
}
