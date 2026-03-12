using CopilotEngineer.Agents;
using CopilotEngineer.Core;

namespace CopilotEngineer.Cli;

public static class CliRunner
{
    public static async Task<string[]> RunAsync(string[] args, CancellationToken cancellationToken = default)
    {
        var input = args.Length == 0
            ? "copilot ask bootstrap status"
            : string.Join(' ', args);

        var engineerCore = EngineerCoreFactory.CreateDefault();
        var response = await engineerCore.ProcessAsync(new UserRequest(input), cancellationToken);

        return
        [
            $"Intent: {response.Intent.Name}",
            $"Source: {response.Source}",
            response.Summary,
            .. response.Recommendations.Select(recommendation => $"- {recommendation}")
        ];
    }
}
