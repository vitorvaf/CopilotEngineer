using CopilotEngineer.Agents;
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
}
