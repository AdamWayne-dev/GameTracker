using Microsoft.Data.Sqlite;

namespace GameTracker.Infrastructure;

public static class GameResultsDbInitialiser
{
    public static async Task InitialiseAsync(string connectionString)
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = @"
        CREATE TABLE IF NOT EXISTS GameResults (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        GameKey TEXT NOT NULL,
        PlayerId TEXT NOT NULL,
        PlayerName TEXT NOT NULL,
        RoundKey TEXT NOT NULL,
        NumericScore INTEGER NOT NULL,
        IsSuccess INTEGER NOT NULL,
        RawMessage TEXT NOT NULL,
        SubmittedAtUtc TEXT NOT NULL,
        UNIQUE(GameKey, RoundKey, PlayerId)
    );";

        await command.ExecuteNonQueryAsync();
    }
}
