using System.Text.RegularExpressions;

namespace GameTracker.Core;

public class WordleSummaryTracker : IGameTracker
{
    public string GameKey => "wordle";

    private static readonly Regex ScoreSegment =
        new(@"(?<tries>[1-6])/6:\s*(?<rest>.*?)(?=(?:[1-6]/6:)|\z)",
            RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex AtName =
        new(@"@(?<name>[^\s@]+)", RegexOptions.Compiled);

    /// <summary>
    /// Determines if the message that is parsed is compatible with the wordle tracker using Regex.
    /// </summary>
    /// <param name="message">The message received from Discord</param>
    /// <returns>True or false, depending on the outcome of the regex match</returns>
    public bool CanHandle(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return false;

        return Regex.IsMatch(message,@"[1-6]/6");
    }
    /// <summary>
    /// Parses the incoming message using Regex and then assigns the various outcomes to the results entry.
    /// </summary>
    /// <param name="message">The message received from Discord</param>
    /// <param name="submittedAtUtc">The date/time the message was submitted on Discord</param>
    /// <returns>The parsed and assigned results</returns>
    public IReadOnlyList<GameResult> Parse(string message, DateTime submittedAtUtc)
    {
        var results = new List<GameResult>();

        if (string.IsNullOrWhiteSpace(message))
            return results;

        var roundDate = submittedAtUtc.Date.AddDays(-1);
        var roundKey = roundDate.ToString("yyyy-MM-dd");

        foreach (Match segment in ScoreSegment.Matches(message))
        {
            if (!int.TryParse(segment.Groups["tries"].Value, out var tries))
                continue;

            var rest = segment.Groups["rest"].Value;

            foreach (Match playerMatch in AtName.Matches(rest))
            {
                var rawName = playerMatch.Groups["name"].Value;
                var canonicalHandle = PlayerIdentityResolver.CanonicalizeHandle(rawName);

                if (string.IsNullOrWhiteSpace(canonicalHandle) || canonicalHandle.All(char.IsDigit))
                    continue;

                results.Add(new GameResult
                {
                    GameKey = GameKey,
                    PlayerId = canonicalHandle,
                    PlayerName = canonicalHandle,
                    RoundKey = roundKey,
                    NumericScore = tries,
                    ScoreType = ScoreType.LowerIsBetter,
                    IsSuccess = true,
                    RawMessage = message,
                    SubmittedAtUtc = submittedAtUtc
                });
            }
        }

        return results;
    }
}