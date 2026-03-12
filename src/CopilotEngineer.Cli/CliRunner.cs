using System.CommandLine;
using CopilotEngineer.Agents;
using CopilotEngineer.Core;

namespace CopilotEngineer.Cli;

public static class CliRunner
{
    public static Task<int> RunAsync(string[] args, CancellationToken cancellationToken = default) =>
        BuildCommand().Parse(args).InvokeAsync();

    public static CliRootCommand BuildCommand(IEngineerCore? engineerCore = null)
    {
        engineerCore ??= EngineerCoreFactory.CreateDefault();

        var rootCommand = new CliRootCommand("Copilot Engineer CLI");

        rootCommand.Subcommands.Add(CreateDebugCommand(engineerCore));
        rootCommand.Subcommands.Add(CreateSqlCommand(engineerCore));
        rootCommand.Subcommands.Add(CreateReviewCommand(engineerCore));
        rootCommand.Subcommands.Add(CreateTestCommand(engineerCore));
        rootCommand.Subcommands.Add(CreateAskCommand(engineerCore));

        return rootCommand;
    }

    private static CliCommand CreateDebugCommand(IEngineerCore engineerCore)
    {
        var promptArgument = new CliArgument<string[]>("prompt")
        {
            Arity = ArgumentArity.OneOrMore
        };

        var command = new CliCommand("debug", "Investiga bugs e stack traces");
        command.Arguments.Add(promptArgument);
        command.SetAction(async parseResult =>
        {
            var prompt = JoinPrompt(parseResult.GetValue(promptArgument));
            await WriteResponseAsync(engineerCore, $"debug {prompt}", CancellationToken.None);
        });

        return command;
    }

    private static CliCommand CreateSqlCommand(IEngineerCore engineerCore)
    {
        var analyzeCommand = new CliCommand("analyze", "Analisa SQL");
        var promptArgument = new CliArgument<string[]>("prompt")
        {
            Arity = ArgumentArity.OneOrMore
        };

        analyzeCommand.Arguments.Add(promptArgument);
        analyzeCommand.SetAction(async parseResult =>
        {
            var prompt = JoinPrompt(parseResult.GetValue(promptArgument));
            await WriteResponseAsync(engineerCore, $"sql analyze {prompt}", CancellationToken.None);
        });

        var sqlCommand = new CliCommand("sql", "Operacoes relacionadas a SQL");
        sqlCommand.Subcommands.Add(analyzeCommand);

        return sqlCommand;
    }

    private static CliCommand CreateReviewCommand(IEngineerCore engineerCore)
    {
        var promptArgument = new CliArgument<string[]>("prompt")
        {
            Arity = ArgumentArity.OneOrMore
        };

        var command = new CliCommand("review", "Revisa codigo");
        command.Arguments.Add(promptArgument);
        command.SetAction(async parseResult =>
        {
            var prompt = JoinPrompt(parseResult.GetValue(promptArgument));
            await WriteResponseAsync(engineerCore, $"review {prompt}", CancellationToken.None);
        });

        return command;
    }

    private static CliCommand CreateTestCommand(IEngineerCore engineerCore)
    {
        var generateCommand = new CliCommand("generate", "Gera ideias de testes");
        var promptArgument = new CliArgument<string[]>("prompt")
        {
            Arity = ArgumentArity.OneOrMore
        };

        generateCommand.Arguments.Add(promptArgument);
        generateCommand.SetAction(async parseResult =>
        {
            var prompt = JoinPrompt(parseResult.GetValue(promptArgument));
            await WriteResponseAsync(engineerCore, $"test generate {prompt}", CancellationToken.None);
        });

        var testCommand = new CliCommand("test", "Operacoes relacionadas a testes");
        testCommand.Subcommands.Add(generateCommand);

        return testCommand;
    }

    private static CliCommand CreateAskCommand(IEngineerCore engineerCore)
    {
        var promptArgument = new CliArgument<string[]>("prompt")
        {
            Arity = ArgumentArity.OneOrMore
        };

        var command = new CliCommand("ask", "Consulta geral ao EngineerCore");
        command.Arguments.Add(promptArgument);
        command.SetAction(async parseResult =>
        {
            var prompt = JoinPrompt(parseResult.GetValue(promptArgument));
            await WriteResponseAsync(engineerCore, $"ask {prompt}", CancellationToken.None);
        });

        return command;
    }

    private static async Task WriteResponseAsync(IEngineerCore engineerCore, string input, CancellationToken cancellationToken)
    {
        var response = await engineerCore.ProcessAsync(new UserRequest(input), cancellationToken);
        Console.Out.WriteLine(MarkdownFormatter.Format(response));
    }

    private static string JoinPrompt(string[]? promptTokens) => string.Join(' ', promptTokens ?? []);
}
