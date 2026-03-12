using CopilotEngineer.Memory;
using CopilotEngineer.Workflows;
using CopilotEngineer.Core;

namespace CopilotEngineer.Agents;

public static class EngineerCoreFactory
{
    public static IEngineerCore CreateDefault()
    {
        IEngineerSpecialist[] specialists =
        [
            new DebugSpecialistAgent(),
            new DatabaseSpecialistAgent(),
            new CodeSpecialistAgent()
        ];

        return new EngineerCore(
            new IntentRouter(),
            new AgentRegistry(specialists),
            new WorkflowExecutor(),
            new ProjectContextProvider());
    }
}
