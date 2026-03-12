namespace CopilotEngineer.Core;

public sealed class SkillRegistry : ISkillRegistry
{
    private readonly Dictionary<string, ISkill> skills = new(StringComparer.OrdinalIgnoreCase);

    public SkillRegistry()
    {
    }

    public SkillRegistry(IEnumerable<ISkill> skills)
    {
        ArgumentNullException.ThrowIfNull(skills);

        foreach (var skill in skills)
        {
            Register(skill);
        }
    }

    public void Register(ISkill skill)
    {
        ArgumentNullException.ThrowIfNull(skill);
        skills[skill.Name] = skill;
    }

    public IReadOnlyCollection<ISkill> List() => skills.Values.ToArray();

    public ISkill Resolve(string skillName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(skillName);

        if (skills.TryGetValue(skillName, out var skill))
        {
            return skill;
        }

        throw new InvalidOperationException($"Nenhuma skill registrada com o nome '{skillName}'.");
    }
}
