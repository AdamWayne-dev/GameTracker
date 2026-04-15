using Discord;
using Discord.WebSocket;
using GameTracker.Core;
using GameTracker.Infrastructure;
using dotenv.net;

namespace GameTracker.Bot;
class Program
{
    private static DiscordSocketClient _client = null!;
    private static GameTrackerService _trackingService = null!;

    public const string connectionString = "Data Source=wordle.db";
    
    static async Task Main()
    {
        SQLitePCL.Batteries.Init();
        DotEnv.Load();

        var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");

        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine("Missing DISCORD_TOKEN");
            return;
        }
        // Config setup
        var config = new DiscordSocketConfig
        {
            GatewayIntents =
                GatewayIntents.Guilds |
                GatewayIntents.GuildMessages |
                GatewayIntents.MessageContent
        };

        _client = new DiscordSocketClient(config);
        await GameResultsDbInitialiser.InitialiseAsync(connectionString);
        var resultRepository = new SqliteGameResultRepository(connectionString);
        // Tracker(s) setup
        _trackingService = new GameTrackerService(
            new List<IGameTracker>
            {
                new WordleSummaryTracker()
            },
            resultRepository);

        
        

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
    /// <summary>
    /// Processes and cleans the message received from Discord, with an additional debugging print to console.
    /// </summary>
    /// <param name="message">The incoming message from Discord</param>
    /// <returns></returns>
    private static async Task OnMessageReceivedAsync(SocketMessage message)
    {
        // ************************  TODO: CHANGE BACK ONCE TESTING IS COMPLETE ****************************

        //if (message.Author.IsBot == false)
        //    return;
        //**************************************************************************************************

        
        // Step 1: clean message
        var cleanedText = DiscordMessageTextHelper.ExpandMentionsToText(message);

        // Step 2: process
        var savedCount = await _trackingService.ProcessMessageAsync(
        cleanedText,
        message.Timestamp.UtcDateTime);

        if (savedCount > 0)
        {
            Console.WriteLine($"Saved {savedCount} result(s).");
        }
    }
}