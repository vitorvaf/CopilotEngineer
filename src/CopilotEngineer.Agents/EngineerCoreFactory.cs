using CopilotEngineer.Memory;
using CopilotEngineer.Workflows;
using CopilotEngineer.Core;

namespace CopilotEngineer.Agents;

public static class EngineerCoreFactory
{
    public static IEngineerCore CreateDefault()
    {
        var repositoryRoot = MemoryPaths.ResolveRepositoryRoot();
        var skillRegistry = SkillRegistration.CreateDefault();
        var memoryService = new MemoryService(
            new ProjectContextLoader(Path.Combine(repositoryRoot, "memory", "project-context.yaml")),
            new ConventionsLoader(Path.Combine(repositoryRoot, "memory", "conventions.yaml")),
            new BugMemoryRepository(Path.Combine(repositoryRoot, "memory", "bug-memory.db")));

        IEngineerSpecialist[] specialists =
        [
            new DebugSpecialistAgent(skillRegistry),
            new DatabaseSpecialistAgent(skillRegistry),
            new CodeSpecialistAgent(skillRegistry)
        ];

        var agentRegistry = new AgentRegistry(specialists);

        return new EngineerCore(
            new IntentRouter(),
            agentRegistry,
            new WorkflowExecutor(agentRegistry),
            memoryService);
    }
}
