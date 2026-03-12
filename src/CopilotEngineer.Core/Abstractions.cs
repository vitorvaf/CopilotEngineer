namespace CopilotEngineer.Core;

public interface IEngineerCore
{
    Task<EngineResponse> ProcessAsync(UserRequest request, CancellationToken cancellationToken = default);
}

public interface ISkill
{
    string Name { get; }

    Task<SkillExecutionResult> ExecuteAsync(UserRequest request, EngineerContext context, CancellationToken cancellationToken = default);
}

public interface IIntentRouter
{
    Intent Route(UserRequest request);
}

public interface IEngineerSpecialist
{
    string Name { get; }

    Task<AgentExecutionResult> ExecuteAsync(UserRequest request, EngineerContext context, CancellationToken cancellationToken = default);
}

public interface IAgentRegistry
{
    void Register(IEngineerSpecialist specialist);

    IEngineerSpecialist Resolve(Intent intent);

    IReadOnlyCollection<IEngineerSpecialist> List();
}

public interface ISkillRegistry
{
    void Register(ISkill skill);

    ISkill Resolve(string skillName);

    IReadOnlyCollection<ISkill> List();
}

public interface IWorkflowExecutor
{
    bool HasWorkflow(Intent intent);

    Task<WorkflowExecutionResult> ExecuteAsync(Intent intent, UserRequest request, EngineerContext context, CancellationToken cancellationToken = default);
}

public interface IContextProvider
{
    Task<EngineerContext> BuildAsync(UserRequest request, Intent intent, CancellationToken cancellationToken = default);
}
