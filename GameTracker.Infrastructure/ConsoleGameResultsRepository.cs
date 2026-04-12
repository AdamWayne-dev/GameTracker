using GameTracker.Core;

namespace GameTracker.Infrastructure;
public class ConsoleGameResultsRepository : IGameResultRepository
{
    public Task SaveResultsAsync(IReadOnlyList<GameResult> results)
    {
        foreach (var result in results)
        {
            Console.WriteLine(
                $"[SAVE] {result.PlayerId} -> {result.NumericScore}({result.RoundKey})"
                );
        }

        return Task.CompletedTask;
    }
}
