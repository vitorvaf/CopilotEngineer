namespace CopilotEngineer.Core;

public sealed class AgentRegistry : IAgentRegistry
{
    private readonly Dictionary<string, IEngineerSpecialist> specialists = new(StringComparer.OrdinalIgnoreCase);

    public AgentRegistry()
    {
    }

    public AgentRegistry(IEnumerable<IEngineerSpecialist> specialists)
    {
        ArgumentNullException.ThrowIfNull(specialists);

        foreach (var specialist in specialists)
        {
            Register(specialist);
        }
    }

    public void Register(IEngineerSpecialist specialist)
    {
        ArgumentNullException.ThrowIfNull(specialist);
        specialists[specialist.Name] = specialist;
    }

    public IReadOnlyCollection<IEngineerSpecialist> List() => specialists.Values.ToArray();

    public IEngineerSpecialist Resolve(Intent intent)
    {
        ArgumentNullException.ThrowIfNull(intent);

        if (specialists.TryGetValue(intent.Specialist, out var specialist))
        {
            return specialist;
        }

        throw new InvalidOperationException($"Nenhum especialista registrado para a intencao '{intent.Name}'.");
    }
}
