using Discord.WebSocket;

namespace GameTracker.Bot;

public static class DiscordMessageTextHelper
{
    /// <summary>
    /// Converts the discords 'Mention' representation of the user into a regular text version, for better representation
    /// </summary>
    /// <param name="message">The message received from Discord</param>
    /// <returns>The converted text (string)</returns>
    public static string ExpandMentionsToText(SocketMessage message)
    {
        var text = message.Content;

        foreach(var user in message.MentionedUsers)
        {
            var displayName = (user as SocketGuildUser)?.DisplayName ?? user.Username;
            var replacement = "@" + displayName;

            text = text.Replace($"<@{user.Id}>", replacement)
                .Replace($"<@!{user.Id}>", replacement);
        }

        return text;
    }
}
