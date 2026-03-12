using CopilotEngineer.Core;

namespace CopilotEngineer.Memory;

public sealed class ProjectContextProvider : IContextProvider
{
    public Task<EngineerContext> BuildAsync(UserRequest request, Intent intent, CancellationToken cancellationToken = default)
    {
        var context = new EngineerContext(
            "Copilot Engineer Pessoal",
            "Orquestrador central com agentes especialistas, memoria estrutural e workflows.",
            ["issue-analysis", "bug-investigation", "sql-analysis"],
            ["read_file", "search_code", "git_diff", "run_dotnet_build", "run_dotnet_tests"]);

        return Task.FromResult(context);
    }
}
