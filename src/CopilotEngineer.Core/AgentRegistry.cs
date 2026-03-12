namespace CopilotEngineer.Core;

public sealed class AgentRegistry(IEnumerable<IEngineerAgent> agents) : IAgentRegistry
{
    private readonly IReadOnlyCollection<IEngineerAgent> agents = agents.ToArray();

    public IReadOnlyCollection<IEngineerAgent> List() => agents;

    public IEngineerAgent Resolve(EngineeringIntent intent)
    {
        var agent = agents.FirstOrDefault(candidate => candidate.CanHandle(intent));
        return agent ?? throw new InvalidOperationException($"Nenhum agente registrado para a intencao '{intent.Type}'.");
    }
}
