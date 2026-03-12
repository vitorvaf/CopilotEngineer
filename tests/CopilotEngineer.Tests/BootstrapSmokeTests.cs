using CopilotEngineer.Agents;
using CopilotEngineer.Cli;
using CopilotEngineer.Core;

namespace CopilotEngineer.Tests;

public static class BootstrapSmokeTests
{
    public static async Task RunAsync(CancellationToken cancellationToken = default)
    {
        var engineerCore = EngineerCoreFactory.CreateDefault();
        var debugResponse = await engineerCore.ProcessAsync(new UserRequest("debug NullReferenceException na CLI"), cancellationToken);
        SimpleAssert.Equal("debug", debugResponse.Intent.Name, "Deve classificar requests de debug.");
        SimpleAssert.Equal("bug-investigation", debugResponse.Source, "Deve acionar o workflow de investigacao.");

        var reviewResponse = await engineerCore.ProcessAsync(new UserRequest("review do bootstrap inicial"), cancellationToken);
        SimpleAssert.Equal("review", reviewResponse.Intent.Name, "Deve classificar review corretamente.");
        SimpleAssert.Equal("CodeSpecialist", reviewResponse.Source, "Deve rotear review para o agente de codigo.");

        await ValidateCliAsync(cancellationToken);
    }

    private static async Task ValidateCliAsync(CancellationToken cancellationToken)
    {
        var fakeCore = new FakeEngineerCore();
        var command = CliRunner.BuildCommand(fakeCore);
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            var debugExitCode = await command.Parse(["debug", "NullReferenceException", "na", "CLI"]).InvokeAsync();
            SimpleAssert.Equal("0", debugExitCode.ToString(), "CLI deve retornar sucesso no comando debug.");
            SimpleAssert.Equal("debug NullReferenceException na CLI", fakeCore.LastInput!, "CLI deve encaminhar debug para o EngineerCore.");
            SimpleAssert.Contains("# debug", writer.ToString(), "CLI deve renderizar cabecalho Markdown.");
            SimpleAssert.Contains("## Recommendations", writer.ToString(), "CLI deve renderizar lista Markdown.");

            writer.GetStringBuilder().Clear();

            var testExitCode = await command.Parse(["test", "generate", "EngineerCore"]).InvokeAsync();
            SimpleAssert.Equal("0", testExitCode.ToString(), "CLI deve retornar sucesso no comando test generate.");
            SimpleAssert.Equal("test generate EngineerCore", fakeCore.LastInput!, "CLI deve encaminhar test generate para o EngineerCore.");
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}

internal sealed class FakeEngineerCore : IEngineerCore
{
    public string? LastInput { get; private set; }

    public Task<EngineResponse> ProcessAsync(UserRequest request, CancellationToken cancellationToken = default)
    {
        LastInput = request.Input;

        return Task.FromResult(new EngineResponse(
            new Intent("debug", "DebugSpecialist", false),
            "Resposta em Markdown.",
            "FakeSource",
            ["Primeira recomendacao.", "Segunda recomendacao."]));
    }
}

internal static class SimpleAssert
{
    public static void Equal(string expected, string actual, string message)
    {
        if (!string.Equals(expected, actual, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"{message} Expected='{expected}', Actual='{actual}'.");
        }
    }

    public static void Contains(string expectedFragment, string actual, string message)
    {
        if (!actual.Contains(expectedFragment, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"{message} Expected fragment='{expectedFragment}', Actual='{actual}'.");
        }
    }
}
