using System.Text.RegularExpressions;

namespace GameTracker.Core;

public static class PlayerIdentityResolver
{
    // This property allows me to correctly reassign handles in discord to a more normalised version due to how discord inconsistently tags players -
    // which causes issues with parsing.
    private static readonly Dictionary<string, string> HandleAliases =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["amy080735"] = "amy",
            ["amy"] = "amy",

            ["ryanbm"] = "ryan",
            ["ryan"] = "ryan",

            ["the_fonz"] = "fonz",
            ["fonz"] = "fonz",

            ["smay"] = "smay",
            ["smay5229"] = "smay",

            ["adam"] = "adam",
        };
    /// <summary>
    /// Normalizes the users discord handle, stripping away any extra text or symbols and then convering it to lower case.
    /// </summary>
    /// <param name="raw">The raw handle</param>
    /// <returns>The converted raw handle</returns>
    public static string NormalizeHandle(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return string.Empty;

        raw = raw.Trim();

        if (raw.StartsWith("@"))
            raw = raw[1..];

        raw = raw.ToLowerInvariant();

        // Remove punctuation / odd chars, keep letters, numbers and underscore
        raw = Regex.Replace(raw, @"[^a-z0-9_]", "");

        return raw;
    }

    /// <summary>
    /// Returns the canonical handle if it exists in the HandleAliases dictionary, otherwise it normalises the handle.
    /// </summary>
    /// <param name="raw"></param>
    /// <returns>The canonical or normalised handle</returns>
    public static string CanonicalizeHandle(string raw)
    {
        var normalized = NormalizeHandle(raw);

        if (string.IsNullOrWhiteSpace(normalized))
            return string.Empty;

        return HandleAliases.TryGetValue(normalized, out var canonical)
            ? canonical
            : normalized;
    }
}