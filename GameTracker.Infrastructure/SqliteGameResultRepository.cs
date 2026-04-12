using Microsoft.Data.Sqlite;
using GameTracker.Core;

namespace GameTracker.Infrastructure;

public class SqliteGameResultRepository: IGameResultRepository 
{
    private readonly string _connectionString;

    public SqliteGameResultRepository(string connectionString)
    {
        _connectionString = connectionString; 
    }

    public async Task SaveResultsAsync(IReadOnlyList<GameResult> results)
    {
        if (results.Count == 0) return;

        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        foreach(var result in results)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = @"
            INSERT OR IGNORE INTO GameResults(
                GameKey,
                PlayerId,
                PlayerName,
                RoundKey,
                NumericScore,
                ScoreType,
                IsSuccess,
                RawMessage,
                SubmittedAtUtc
            )
            VALUES (
                $gameKey,
                $playerId,
                $playerName,
                $roundKey,
                $numericScore,
                $scoreType,
                $isSuccess,
                $rawMessage,
                $submittedAtUtc
            );";

            command.Parameters.AddWithValue("$gameKey", result.GameKey);
            command.Parameters.AddWithValue("$playerId", result.PlayerId);
            command.Parameters.AddWithValue("$playerName", result.PlayerName);
            command.Parameters.AddWithValue("$roundKey", result.RoundKey);

            if (result.NumericScore.HasValue)
                command.Parameters.AddWithValue("$numericScore", result.NumericScore.Value);
            else
                command.Parameters.AddWithValue("$numericScore", DBNull.Value);

            command.Parameters.AddWithValue("$scoreType", (int)result.ScoreType);
            command.Parameters.AddWithValue("$isSuccess", result.IsSuccess ? 1 : 0);
            command.Parameters.AddWithValue("$rawMessage", result.RawMessage);
            command.Parameters.AddWithValue("$submittedAtUtc", result.SubmittedAtUtc.ToString("O"));

            await command.ExecuteNonQueryAsync();
        }
    }
}
