namespace GameTracker.Core;

public interface IGameResultRepository
{
    Task SaveResultsAsync(IReadOnlyList<GameResult> results);
}
