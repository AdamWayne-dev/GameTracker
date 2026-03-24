namespace GameTracker.Core;

public class GameResult
{
    public string GameKey { get; set; } = "";
    public string PlayerId { get; set; } = "";
    public string PlayerName { get; set; } = "";

    public string RoundKey { get; set; } = "";
    public int? NumericScore { get; set; }

    public ScoreType ScoreType { get; set; }
    public bool IsSuccess { get; set; }

    public string RawMessage { get; set; } = "";
    public DateTime SubmittedAtUtc { get; set; }
}

public enum ScoreType
{
    LowerIsBetter,
    HigherIsBetter,
    WinLoss,
    TimeBased
}
