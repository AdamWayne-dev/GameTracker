namespace GameTracker.Core;

public interface IGameTracker
{
    string GameKey { get; }

    bool CanHandle(string message);

    IReadOnlyList<GameResult> Parse(
        string message,
        DateTime submittedAtUtc);
}
