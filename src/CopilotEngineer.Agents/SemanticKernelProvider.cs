using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using CopilotEngineer.Core;

namespace CopilotEngineer.Agents;

public sealed class SemanticKernelProvider : ILLMProvider
{
    private readonly IChatCompletionService chatCompletionService;

    public SemanticKernelProvider(string modelId, string apiKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modelId);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);

        var kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.AddOpenAIChatCompletion(modelId, apiKey);

        var kernel = kernelBuilder.Build();
        chatCompletionService = kernel.Services.GetRequiredService<IChatCompletionService>();
    }

    public async Task<LlmCompletionResponse> GenerateAsync(LlmCompletionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var history = new ChatHistory();
        history.AddSystemMessage(request.SystemPrompt);
        history.AddUserMessage(request.UserPrompt);

        var settings = new OpenAIPromptExecutionSettings
        {
            Temperature = request.Temperature
        };

        var response = await chatCompletionService.GetChatMessageContentAsync(history, settings, cancellationToken: cancellationToken);
        var content = response.Content?.Trim();

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidOperationException("Semantic Kernel retornou uma resposta vazia.");
        }

        return new LlmCompletionResponse(content);
    }

    public static SemanticKernelProvider CreateFromEnvironment()
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        var modelId = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-4.1-mini";

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "OPENAI_API_KEY nao configurada. Defina OPENAI_API_KEY e opcionalmente OPENAI_MODEL para usar o EngineerCore com Semantic Kernel.");
        }

        return new SemanticKernelProvider(modelId, apiKey);
    }
}
