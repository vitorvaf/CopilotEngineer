using CopilotEngineer.Agents;
using CopilotEngineer.Core;
using CopilotEngineer.Memory;
using CopilotEngineer.Workflows;

namespace CopilotEngineer.Cli;

public static class CliRunner
{
    public static async Task<string[]> RunAsync(string[] args, CancellationToken cancellationToken = default)
    {
        var input = args.Length == 0
            ? "copilot ask bootstrap status"
            : string.Join(' ', args);

        var agents = new IEngineerAgent[]
        {
            new DebugSpecialistAgent(),
            new DatabaseSpecialistAgent(),
            new CodeSpecialistAgent()
        };

        var engineerCore = new EngineerCore(
            new IntentRouter(),
            new AgentRegistry(agents),
            new WorkflowExecutor(),
            new ProjectContextProvider());

        var response = await engineerCore.ProcessAsync(new EngineerRequest(input), cancellationToken);

        return
        [
            $"Intent: {response.Intent.Type}",
            $"Source: {response.Source}",
            response.Summary,
            .. response.Recommendations.Select(recommendation => $"- {recommendation}")
        ];
    }
}
