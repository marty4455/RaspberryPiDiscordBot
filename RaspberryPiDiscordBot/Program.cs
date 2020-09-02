using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Services.Description;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using RaspberryPiDiscordBot.Config;
using RaspberryPiDiscordBot.Models;

namespace RaspberryPiDiscordBot
{
    public class Program
    {
        private DiscordSocketClient _client;

        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            // When working with events that have Cacheable<IMessage, ulong> parameters,
            // you must enable the message cache in your config settings if you plan to
            // use the cached message entity. 
            var _config = new DiscordSocketConfig { MessageCacheSize = 100 };
            _client = new DiscordSocketClient(_config);

            var configTokenPath = new ConfigPath().Path;
            var token = JsonConvert.DeserializeObject<TokenConfig>(File.ReadAllText(configTokenPath)).Token;
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.MessageReceived += MessageReceived; 
            _client.MessageUpdated += MessageUpdated;
            _client.Ready += () =>
            {
                Console.WriteLine("Bot is connected!");
                return Task.CompletedTask;
            };
		
            await Task.Delay(-1);
        }

        private async Task MessageReceived(SocketMessage message)
        {
            if (!message.Content.StartsWith("-")) return;
            if (message.Content == "-ping")
            {
                await message.Channel.SendMessageAsync("Pong op je hoofd");
            }
        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            // If the message was not in the cache, downloading it will result in getting a copy of `after`.
            var message = await before.GetOrDownloadAsync();
            Console.WriteLine($"{message} -> {after}");
        }
    }
}