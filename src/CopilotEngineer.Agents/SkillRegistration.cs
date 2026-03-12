using CopilotEngineer.Core;

namespace CopilotEngineer.Agents;

public static class SkillRegistration
{
    public static SkillRegistry CreateDefault()
    {
        ISkill[] skills =
        [
            new AnalyzeStackTraceSkill(),
            new AnalyzeSqlQuerySkill(),
            new ReviewCodeSkill(),
            new GenerateTestsSkill()
        ];

        return new SkillRegistry(skills);
    }
}
