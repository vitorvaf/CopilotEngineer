using CopilotEngineer.Agents;
using CopilotEngineer.Cli;
using CopilotEngineer.Core;
using CopilotEngineer.Memory;
using Xunit;

namespace CopilotEngineer.Tests;

public sealed class BootstrapSmokeTests
{
    [Fact]
    public async Task EngineerCoreRoutesBootstrapRequestsCorrectly()
    {
        var engineerCore = EngineerCoreFactory.CreateDefault();
        var issueResponse = await engineerCore.ProcessAsync(new UserRequest("implementar feature de exportacao CSV"));
        Assert.Equal("issue", issueResponse.Intent.Name);
        Assert.Equal("issue-analysis", issueResponse.Source);

        var debugResponse = await engineerCore.ProcessAsync(new UserRequest("debug NullReferenceException na CLI"));
        Assert.Equal("debug", debugResponse.Intent.Name);
        Assert.Equal("bug-investigation", debugResponse.Source);

        var sqlResponse = await engineerCore.ProcessAsync(new UserRequest("sql select * from orders"));
        Assert.Equal("sql", sqlResponse.Intent.Name);
        Assert.Equal("sql-analysis", sqlResponse.Source);

        var reviewResponse = await engineerCore.ProcessAsync(new UserRequest("review do bootstrap inicial"));
        Assert.Equal("review", reviewResponse.Intent.Name);
        Assert.Equal("CodeSpecialist", reviewResponse.Source);
    }

    [Fact]
    public async Task CliForwardsCommandsToEngineerCore()
    {
        var fakeCore = new FakeEngineerCore();
        var command = CliRunner.BuildCommand(fakeCore);
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            var debugExitCode = await command.Parse(["debug", "NullReferenceException", "na", "CLI"]).InvokeAsync();
            Assert.Equal(0, debugExitCode);
            Assert.Equal("debug NullReferenceException na CLI", fakeCore.LastInput);
            Assert.Contains("# debug", writer.ToString());
            Assert.Contains("## Recommendations", writer.ToString());

            writer.GetStringBuilder().Clear();

            var testExitCode = await command.Parse(["test", "generate", "EngineerCore"]).InvokeAsync();
            Assert.Equal(0, testExitCode);
            Assert.Equal("test generate EngineerCore", fakeCore.LastInput);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public async Task MemoryServiceLoadsYamlAndSearchesBugMemories()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), $"copilot-engineer-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempRoot);

        try
        {
            var memoryDirectory = Path.Combine(tempRoot, "memory");
            Directory.CreateDirectory(memoryDirectory);

            await File.WriteAllTextAsync(
                Path.Combine(memoryDirectory, "project-context.yaml"),
                """
                project:
                  name: Copilot Engineer Pessoal
                  architecture: Engineer Core + Specialist Agents + Memory + Workflow Engine
                  stack:
                    - .NET 8
                    - Semantic Kernel
                  workflows:
                    - bug-investigation
                  tools:
                    - run_dotnet_tests
                """);

            await File.WriteAllTextAsync(
                Path.Combine(memoryDirectory, "conventions.yaml"),
                """
                conventions:
                  naming:
                    classes: PascalCase
                    interfaces: I*
                  patterns:
                    - dependency-injection
                """);

            var repository = new BugMemoryRepository(Path.Combine(memoryDirectory, "bug-memory.db"));
            await repository.AddAsync(
                "NullReferenceException na CLI",
                "Erro ao formatar a saida do comando debug.",
                "Objeto de resposta sem validacao nula.",
                "Adicionar validacao antes de renderizar markdown.",
                ["cli", "debug", "nullreference"]);

            var service = new MemoryService(
                new ProjectContextLoader(Path.Combine(memoryDirectory, "project-context.yaml")),
                new ConventionsLoader(Path.Combine(memoryDirectory, "conventions.yaml")),
                repository);

            var context = await service.BuildAsync(new UserRequest("debug NullReferenceException na CLI"), Intent.Debug);

            Assert.Equal("Copilot Engineer Pessoal", context.ProjectName);
            Assert.Contains(".NET 8", context.ProjectStack);
            Assert.Equal("PascalCase", context.NamingConventions["classes"]);
            Assert.Contains("dependency-injection", context.DesignPatterns);
            Assert.Single(context.RelatedBugMemories);
            Assert.Equal("NullReferenceException na CLI", context.RelatedBugMemories.First().Title);
        }
        finally
        {
            Directory.Delete(tempRoot, recursive: true);
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
