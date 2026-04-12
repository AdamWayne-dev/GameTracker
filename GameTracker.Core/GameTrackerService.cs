namespace GameTracker.Core;

public class GameTrackerService
{
    private readonly IReadOnlyList<IGameTracker> _trackers;
    private readonly IGameResultRepository _repository;

    public GameTrackerService(IEnumerable<IGameTracker> trackers, IGameResultRepository repository)
    {
        _trackers = trackers?.ToList()
            ?? throw new ArgumentNullException(nameof(trackers));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        
    }
    /// <summary>
    /// The received message is filtered through each tracker to determine if it is able to handle the format.
    /// </summary>
    /// <param name="message">The message received from Discord</param>
    /// <param name="submittedAtUTC">The time the message was submitted</param>
    /// <returns>The parsed result and the time it was submitted (in Discord).</returns>
    public async Task<int> ProcessMessageAsync(string message, DateTime submittedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(message))
            return 0;

        var tracker = _trackers.FirstOrDefault(t => t.CanHandle(message));

        if (tracker is null)
            return 0;

        var results = tracker.Parse(message, submittedAtUtc);

        if (results.Count == 0)
            return 0;

        await _repository.SaveResultsAsync(results);

        return results.Count;
    }
}
