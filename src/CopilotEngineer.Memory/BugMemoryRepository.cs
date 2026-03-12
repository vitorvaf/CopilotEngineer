using CopilotEngineer.Core;
using Microsoft.Data.Sqlite;

namespace CopilotEngineer.Memory;

public sealed class BugMemoryRepository(string databasePath)
{
    public string DatabasePath { get; } = databasePath;

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(DatabasePath) ?? ".");

        await using var connection = await OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS bug_memories (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                title TEXT NOT NULL,
                summary TEXT NOT NULL,
                root_cause TEXT NOT NULL,
                resolution TEXT NOT NULL,
                tags TEXT NOT NULL,
                created_at_utc TEXT NOT NULL
            );
            """;

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<long> AddAsync(
        string title,
        string summary,
        string rootCause,
        string resolution,
        IReadOnlyCollection<string>? tags = null,
        CancellationToken cancellationToken = default)
    {
        await InitializeAsync(cancellationToken);

        await using var connection = await OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO bug_memories (title, summary, root_cause, resolution, tags, created_at_utc)
            VALUES ($title, $summary, $rootCause, $resolution, $tags, $createdAtUtc);
            SELECT last_insert_rowid();
            """;
        command.Parameters.AddWithValue("$title", title);
        command.Parameters.AddWithValue("$summary", summary);
        command.Parameters.AddWithValue("$rootCause", rootCause);
        command.Parameters.AddWithValue("$resolution", resolution);
        command.Parameters.AddWithValue("$tags", SerializeTags(tags));
        command.Parameters.AddWithValue("$createdAtUtc", DateTimeOffset.UtcNow.ToString("O"));

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt64(result, System.Globalization.CultureInfo.InvariantCulture);
    }

    public async Task<IReadOnlyCollection<BugMemoryRecord>> SearchRelevantAsync(
        string query,
        int limit = 5,
        CancellationToken cancellationToken = default)
    {
        await InitializeAsync(cancellationToken);

        var searchTerms = query
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(static term => term.Length >= 4)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(4)
            .ToArray();

        await using var connection = await OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();

        if (searchTerms.Length == 0)
        {
            command.CommandText =
                """
                SELECT id, title, summary, root_cause, resolution, tags, created_at_utc
                FROM bug_memories
                ORDER BY datetime(created_at_utc) DESC
                LIMIT $limit;
                """;
        }
        else
        {
            var conditions = new List<string>(searchTerms.Length);

            for (var index = 0; index < searchTerms.Length; index++)
            {
                var parameterName = $"$term{index}";
                conditions.Add($"(title LIKE {parameterName} OR summary LIKE {parameterName} OR root_cause LIKE {parameterName} OR tags LIKE {parameterName})");
                command.Parameters.AddWithValue(parameterName, $"%{searchTerms[index]}%");
            }

            command.CommandText =
                $"""
                 SELECT id, title, summary, root_cause, resolution, tags, created_at_utc
                 FROM bug_memories
                 WHERE {string.Join(" OR ", conditions)}
                 ORDER BY datetime(created_at_utc) DESC
                 LIMIT $limit;
                 """;
        }

        command.Parameters.AddWithValue("$limit", limit);

        var results = new List<BugMemoryRecord>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new BugMemoryRecord(
                reader.GetInt64(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                DeserializeTags(reader.GetString(5)),
                DateTimeOffset.Parse(reader.GetString(6), System.Globalization.CultureInfo.InvariantCulture)));
        }

        return results;
    }

    private async Task<SqliteConnection> OpenConnectionAsync(CancellationToken cancellationToken)
    {
        var connection = new SqliteConnection($"Data Source={DatabasePath}");
        await connection.OpenAsync(cancellationToken);
        return connection;
    }

    private static string SerializeTags(IReadOnlyCollection<string>? tags) =>
        string.Join(',', tags ?? []);

    private static IReadOnlyCollection<string> DeserializeTags(string tags) =>
        tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
