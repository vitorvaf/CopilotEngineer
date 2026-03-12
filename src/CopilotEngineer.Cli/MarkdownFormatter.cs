using CopilotEngineer.Core;

namespace CopilotEngineer.Cli;

internal static class MarkdownFormatter
{
    public static string Format(EngineResponse response)
    {
        var lines = new List<string>
        {
            $"# {response.Intent.Name}",
            string.Empty,
            $"**Source:** `{response.Source}`",
            string.Empty,
            response.Summary
        };

        if (response.Recommendations.Count > 0)
        {
            lines.Add(string.Empty);
            lines.Add("## Recommendations");

            foreach (var recommendation in response.Recommendations)
            {
                lines.Add($"- {recommendation}");
            }
        }

        return string.Join(Environment.NewLine, lines);
    }
}
