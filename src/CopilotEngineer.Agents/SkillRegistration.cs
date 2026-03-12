using CopilotEngineer.Core;

namespace CopilotEngineer.Agents;

public static class SkillRegistration
{
    public static SkillRegistry CreateDefault(ILLMService llmService)
    {
        ISkill[] skills =
        [
            new AnalyzeStackTraceSkill(llmService),
            new AnalyzeSqlQuerySkill(llmService),
            new ReviewCodeSkill(llmService),
            new GenerateTestsSkill(llmService)
        ];

        return new SkillRegistry(skills);
    }
}
