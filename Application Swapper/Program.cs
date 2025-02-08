using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Application_Swapper
{
    class Program
    {
        private static async Task Main(string[] args) => await new Program().RunBotAsync();

        private async Task RunBotAsync()
        {
            var client = new DiscordSocketClient(new DiscordSocketConfig
            {
                // gets the data from Discord Developer portal for permissions
                GatewayIntents = GatewayIntents.MessageContent | GatewayIntents.Guilds | GatewayIntents.GuildMessages
            });

            client.Log += LogAsync;
            client.MessageReceived += MessageReceivedAsync;
            client.Ready += ReadyAsync;
            client.MessageReceived += CommandReceivedAsync;

            // bot token. DO NOT. SHARE THIS. 1 key has been randomized on upload to make sure there's no issues.
            string token = "bot token";

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            // keep program running until closed
            await Task.Delay(-1);
        }

        private async Task CommandReceivedAsync(SocketMessage arg)
        {
            // ignore messages from the bot itself
            if (arg is not SocketUserMessage message) return;
            if (message.Author.IsBot) return;

            // make sure message is correct and not from a dm/somewhere the bot cannot access
            if (message.Channel is SocketTextChannel textChannel)
            {
                if (message.Content.StartsWith("!approve"))
                {
                    ulong sourceChannelId = 1337607589571072081;  // channel id of mod channe
                    if (textChannel.Id == sourceChannelId)
                    {
                        ulong targetChannelId = 1337619585418924093;  // channel id of application channel

                        // double check channel id
                        var targetChannel = textChannel.Guild.GetTextChannel(targetChannelId);

                        if (targetChannel != null)
                        {
                            string messageContentWithoutCommand = message.Content.Substring("!approve ".Length).Trim();

                            // send message without the command part
                            await targetChannel.SendMessageAsync($"Full approval issued for: {messageContentWithoutCommand}");
                        }
                        else
                        {
                            // if the channel id is incorrect
                            Console.WriteLine("Target channel not found.");
                        }
                    }
                }
                else
                {
                    // error handling
                    Console.WriteLine("Message error. Please try again.");
                }

            }
        }

        private Task LogAsync(LogMessage log)
        {
            // show the status of the bot (connecting, ready, etc)
            Console.WriteLine($"Log: {log}");
            return Task.CompletedTask;
        }

        private async Task ReadyAsync()
        {
            // if connection is successful
            Console.WriteLine("Bot is online and ready!");
        }

        private async Task MessageReceivedAsync(SocketMessage arg)
        {
            // ignore messages from the bot itself
            if (arg is not SocketUserMessage message) return;
            if (message.Author.IsBot) return;

            // debug; same message to be sent into mod chat
            Console.WriteLine($"New Application from: {message.Author.Username}\nMessage Contents: {message.Content}");

            // make sure message is correct and not from a dm/somewhere the bot cannot access
            if (message.Channel is SocketTextChannel textChannel)
            {
                ulong sourceChannelId = 1337619585418924093;  // channel id of application channel
                if (textChannel.Id == sourceChannelId)
                {
                    ulong targetChannelId = 1337607589571072081;  // channel id of mod channel

                    // double check channel id
                    var targetChannel = textChannel.Guild.GetTextChannel(targetChannelId);

                    if (targetChannel != null)
                    {
                        // send message
                        await targetChannel.SendMessageAsync($"New Application from: {message.Author.Username}\nMessage Contents: {message.Content}");
                    }
                    else
                    {
                        // if the channel id is incorrect
                        Console.WriteLine("Target channel not found.");
                    }
                }
            }
            else
            {
                // error handling
                Console.WriteLine("Message error. Please try again.");
            }

        }
    }
}
