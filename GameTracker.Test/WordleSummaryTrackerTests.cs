using GameTracker.Core;
using Xunit;

namespace GameTracker.Tests;

public class WordleSummaryTrackerTests
{
    [Fact]
    public void CanHandle_ReturnsTrue_ForSummaryMessage()
    {
        var tracker = new WordleSummaryTracker();

        var message = """
            3/6: @Adam @Amy
            4/6: @Ryan @Smay @fonz
            """;

        var result = tracker.CanHandle(message);

        Assert.True(result);
    }

    [Fact]
    public void Parse_ReturnsExpectedResults()
    {
        var tracker = new WordleSummaryTracker();

        var message = """
            2/6: @Smay @Adam @Amy @Ryan
            3/6: @fonz
            """;

        var submittedAtUtc = new DateTime(2026, 03, 30, 12, 0, 0, DateTimeKind.Utc);

        var results = tracker.Parse(message, submittedAtUtc);

        Assert.Equal(5, results.Count());
        Assert.Contains(results, r => r.PlayerId == "smay" && r.NumericScore == 2 && r.RoundKey == "2026-03-29");
        Assert.Contains(results, r => r.PlayerId == "adam" && r.NumericScore == 2 && r.RoundKey == "2026-03-29");
        Assert.Contains(results, r => r.PlayerId == "amy" && r.NumericScore == 2 && r.RoundKey == "2026-03-29");
        Assert.Contains(results, r => r.PlayerId == "ryan" && r.NumericScore == 2 && r.RoundKey == "2026-03-29");
        Assert.Contains(results, r => r.PlayerId == "fonz" && r.NumericScore == 3 && r.RoundKey == "2026-03-29");
    }
}
