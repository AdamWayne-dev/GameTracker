using System.Text.RegularExpressions;

namespace GameTracker.Core;

public class WordleSummaryTracker : IGameTracker
{
    public string GameKey => "wordle";

    // Matches sections like:
    // 2/6: @Ryan
    // 3/6: @fonz @Amy @Smay
    private static readonly Regex ScoreSegment =
        new(@"(?<tries>[1-6])/6:\s*(?<rest>.*?)(?=(?:[1-6]/6:)|\z)",
            RegexOptions.Compiled | RegexOptions.Singleline);

    // Matches @name tokens in the segment
    private static readonly Regex AtName =
        new(@"@(?<name>[^\s@]+)", RegexOptions.Compiled);

    // Tries to find the Wordle number in the recap
    private static readonly Regex PuzzleNumberRegex =
        new(@"Wordle\s+(?:No\.\s*)?(?<puzzle>\d+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public bool CanHandle(string message)
    {
        return message.Contains("Wordle", StringComparison.OrdinalIgnoreCase)
            && message.Contains("/6:");
    }

    public IReadOnlyList<GameResult> Parse(string message, DateTime submittedAtUtc)
    {
        var results = new List<GameResult>();

        var puzzleMatch = PuzzleNumberRegex.Match(message);
        if (!puzzleMatch.Success)
            return results;

        var roundKey = puzzleMatch.Groups["puzzle"].Value;

        foreach (Match segment in ScoreSegment.Matches(message))
        {
            var tries = int.Parse(segment.Groups["tries"].Value);
            var rest = segment.Groups["rest"].Value;

            foreach (Match playerMatch in AtName.Matches(rest))
            {
                var rawName = playerMatch.Groups["name"].Value;
                var canonicalHandle = PlayerIdentityResolver.CanonicalizeHandle(rawName);

                if (string.IsNullOrWhiteSpace(canonicalHandle))
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