namespace GameTracker.Core;

public class GameTrackerService
{
    private readonly IReadOnlyList<IGameTracker> _trackers;

    public GameTrackerService(IEnumerable<IGameTracker> trackers)
    {
        _trackers = trackers?.ToList()
            ?? throw new ArgumentNullException(nameof(trackers));
    }

    public IReadOnlyList<GameResult> ProcessMessage(string message, DateTime submittedAtUTC)
    {
        if (string.IsNullOrWhiteSpace(message))
            return [];

        var tracker = _trackers.FirstOrDefault(t => t.CanHandle(message));

        if (tracker is null)
            return [];

        return tracker.Parse(message, submittedAtUTC);
    }
}
