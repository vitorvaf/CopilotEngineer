using CopilotEngineer.Memory;
using CopilotEngineer.Workflows;
using CopilotEngineer.Core;

namespace CopilotEngineer.Agents;

public static class EngineerCoreFactory
{
    public static IEngineerCore CreateDefault()
    {
        var skillRegistry = SkillRegistration.CreateDefault();

        IEngineerSpecialist[] specialists =
        [
            new DebugSpecialistAgent(skillRegistry),
            new DatabaseSpecialistAgent(skillRegistry),
            new CodeSpecialistAgent(skillRegistry)
        ];

        return new EngineerCore(
            new IntentRouter(),
            new AgentRegistry(specialists),
            new WorkflowExecutor(),
            new ProjectContextProvider());
    }
}
