using System.Text;
using System.Text.Json;

namespace CopilotEngineer.Core;

public sealed class LLMService(ILLMProvider provider) : ILLMService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false
    };

    public async Task<SkillExecutionResult> ExecuteSkillAsync(
        string skillName,
        string specialistName,
        string instructions,
        UserRequest request,
        EngineerContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(skillName);
        ArgumentException.ThrowIfNullOrWhiteSpace(specialistName);
        ArgumentException.ThrowIfNullOrWhiteSpace(instructions);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(context);

        var completion = await provider.GenerateAsync(
            new LlmCompletionRequest(
                BuildSystemPrompt(skillName, specialistName, instructions),
                BuildUserPrompt(request, context)),
            cancellationToken);

        return ParseSkillResult(skillName, completion.Content);
    }

    private static string BuildSystemPrompt(string skillName, string specialistName, string instructions)
    {
        var prompt = new StringBuilder();
        prompt.AppendLine($"Voce atua como a skill '{skillName}' do agente '{specialistName}'.");
        prompt.AppendLine(instructions);
        prompt.AppendLine("Responda em portugues do Brasil.");
        prompt.AppendLine("Retorne somente JSON valido sem markdown nem comentarios.");
        prompt.AppendLine("""Use o formato: {"summary":"texto curto","outputs":["item 1","item 2","item 3"]}.""");
        prompt.AppendLine("`summary` deve sintetizar a analise em 1 ou 2 frases.");
        prompt.AppendLine("`outputs` deve conter de 2 a 5 acoes, achados ou recomendacoes objetivas.");
        return prompt.ToString();
    }

    private static string BuildUserPrompt(UserRequest request, EngineerContext context)
    {
        var payload = new
        {
            request = request.Input,
            metadata = request.Metadata ?? new Dictionary<string, string>(),
            project = new
            {
                name = context.ProjectName,
                architecture = context.ArchitectureSummary,
                workflows = context.AvailableWorkflows,
                tools = context.AvailableTools,
                stack = context.ProjectStack,
                namingConventions = context.NamingConventions,
                designPatterns = context.DesignPatterns
            },
            relatedBugMemories = context.RelatedBugMemories.Select(memory => new
            {
                memory.Title,
                memory.Summary,
                memory.RootCause,
                memory.Resolution,
                memory.Tags
            })
        };

        return JsonSerializer.Serialize(payload, JsonOptions);
    }

    private static SkillExecutionResult ParseSkillResult(string skillName, string rawContent)
    {
        var sanitized = StripCodeFences(rawContent);

        try
        {
            using var document = JsonDocument.Parse(sanitized);
            var root = document.RootElement;
            var summary = root.TryGetProperty("summary", out var summaryElement)
                ? summaryElement.GetString()
                : null;

            var outputs = root.TryGetProperty("outputs", out var outputsElement) && outputsElement.ValueKind == JsonValueKind.Array
                ? outputsElement.EnumerateArray()
                    .Select(item => item.GetString())
                    .Where(item => !string.IsNullOrWhiteSpace(item))
                    .Cast<string>()
                    .ToArray()
                : [];

            if (!string.IsNullOrWhiteSpace(summary) && outputs.Length > 0)
            {
                return new SkillExecutionResult(skillName, summary, outputs);
            }
        }
        catch (JsonException)
        {
        }

        var fallback = sanitized.Trim();
        return new SkillExecutionResult(
            skillName,
            fallback,
            [fallback]);
    }

    private static string StripCodeFences(string content)
    {
        var trimmed = content.Trim();
        if (!trimmed.StartsWith("```", StringComparison.Ordinal))
        {
            return trimmed;
        }

        var lines = trimmed.Split(Environment.NewLine);
        if (lines.Length <= 2)
        {
            return trimmed.Trim('`');
        }

        return string.Join(Environment.NewLine, lines.Skip(1).SkipLast(1)).Trim();
    }
}
