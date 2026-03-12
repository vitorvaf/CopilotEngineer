namespace CopilotEngineer.Core;

public sealed class IntentRouter : IIntentRouter
{
    public Intent Route(UserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Input);

        var normalized = request.Input.Trim().ToLowerInvariant();

        if (normalized.Contains("sql", StringComparison.Ordinal))
        {
            return Intent.Sql;
        }

        if (normalized.Contains("debug", StringComparison.Ordinal) || normalized.Contains("erro", StringComparison.Ordinal))
        {
            return Intent.Debug;
        }

        if (normalized.Contains("review", StringComparison.Ordinal) || normalized.Contains("refactor", StringComparison.Ordinal))
        {
            return Intent.Review;
        }

        return Intent.Ask;
    }
}
