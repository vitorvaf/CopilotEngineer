namespace CopilotEngineer.Core;

public sealed class IntentRouter : IIntentRouter
{
    public EngineeringIntent Route(string request)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request);

        var normalized = request.Trim().ToLowerInvariant();

        if (normalized.Contains("sql", StringComparison.Ordinal))
        {
            return EngineeringIntent.Sql;
        }

        if (normalized.Contains("debug", StringComparison.Ordinal) || normalized.Contains("erro", StringComparison.Ordinal))
        {
            return EngineeringIntent.Debug;
        }

        if (normalized.Contains("review", StringComparison.Ordinal) || normalized.Contains("refactor", StringComparison.Ordinal))
        {
            return EngineeringIntent.Review;
        }

        return EngineeringIntent.Ask;
    }
}
