using Discord;
using Discord.WebSocket;
using GameTracker.Bot;
using GameTracker.Core;
using dotenv.net;

class Program
{
    private static DiscordSocketClient _client = null!;
    private static GameTrackerService _trackingService = null!;

    static async Task Main()
    {
        DotEnv.Load();

        var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");

        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine("Missing DISCORD_TOKEN");
            return;
        }

        var config = new DiscordSocketConfig
        {
            GatewayIntents =
                GatewayIntents.Guilds |
                GatewayIntents.GuildMessages |
                GatewayIntents.MessageContent
        };

        _client = new DiscordSocketClient(config);

        // Setup trackers
        _trackingService = new GameTrackerService(
            new List<IGameTracker>
            {
                new WordleSummaryTracker()
            });

        _client.Log += msg =>
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        };

        _client.MessageReceived += OnMessageReceivedAsync;


        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        Console.WriteLine("Bot running...");
        await Task.Delay(-1);
    }

    private static async Task OnMessageReceivedAsync(SocketMessage message)
    {
        // ************************  TODO: CHANGE BACK ONCE TESTING IS COMPLETE ****************************

        //if (message.Author.IsBot == false)
        //    return;
        //**************************************************************************************************

        
        // Step 1: clean message
        var cleanedText = DiscordMessageTextHelper.ExpandMentionsToText(message);

        // Step 2: process
        var results = _trackingService.ProcessMessage(
            cleanedText,
            message.Timestamp.UtcDateTime);

        // Step 3: debug output
        foreach (var result in results)
        {
            Console.WriteLine(
                $"{result.PlayerId} -> {result.NumericScore} ({result.RoundKey})");
        }
    }
}