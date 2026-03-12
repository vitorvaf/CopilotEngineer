namespace CopilotEngineer.McpClients;

public sealed class McpServerCatalog
{
    public IReadOnlyCollection<string> ListServers() =>
        ["filesystem", "git", "sqlite", "docker", "dotnet"];
}
