using Discord.WebSocket;

namespace GameTracker.Bot;

public static class DiscordMessageTextHelper
{
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
