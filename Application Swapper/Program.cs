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
            client.Ready += ReadyAsync;
            client.MessageReceived += MessageReceivedAsync;
            client.MessageReceived += CommandReceivedAsync;

            // bot token. 
            string token = "token";

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            // keep program running until closed
            await Task.Delay(-1);
        }

        private async Task CommandReceivedAsync(SocketMessage arg)
        {
            ulong sourceChannelId = 1337883290463371304;  // channel id of mod channel
            ulong targetChannelId = 1335950685774155806;  // channel id of application channel
                                                          // ignore messages from the bot itself
            if (arg is not SocketUserMessage message) return;
            if (message.Author.IsBot) return;

            // make sure message is correct and not from a dm/somewhere the bot cannot access
            if (message.Channel is SocketTextChannel textChannel)
            {
                if (message.Content.StartsWith("/approve"))
                {

                    if (textChannel.Id == sourceChannelId)
                    {
                        // double check channel id
                        var targetChannel = textChannel.Guild.GetTextChannel(targetChannelId);
                        var sourceChannel = textChannel.Guild.GetTextChannel(sourceChannelId);

                        if (targetChannel != null)
                        {
                            string messageContentWithoutCommand = message.Content.Substring("/approve ".Length).Trim();

                            // send message without the command part
                            await targetChannel.SendMessageAsync($"Full approval issued for: {messageContentWithoutCommand}");
                            await sourceChannel.SendMessageAsync("Approval message sent.");
                        }
                        else
                        {
                            // if the channel id is incorrect
                            Console.WriteLine("Target channel not found.");
                        }
                    }
                }

                else if (message.Content.StartsWith("/halfapprove"))
                {
                    if (textChannel.Id == sourceChannelId)
                    {
                        // double check channel id
                        var targetChannel = textChannel.Guild.GetTextChannel(targetChannelId);

                        if (targetChannel != null)
                        {
                            string messageContentWithoutCommand = message.Content.Substring("/halfapprove ".Length).Trim();

                            // send message without the command part
                            await targetChannel.SendMessageAsync($"One approval has been issued for: {messageContentWithoutCommand}");

                        }
                        else
                        {
                            // if the channel id is incorrect
                            Console.WriteLine("Target channel not found.");
                        }
                    }
                }
                else if (message.Content.StartsWith("/review"))
                {
                    if (textChannel.Id == sourceChannelId)
                    {
                        // double check channel id
                        var targetChannel = textChannel.Guild.GetTextChannel(targetChannelId);

                        if (targetChannel != null)
                        {
                            string messageContentWithoutCommand = message.Content.Substring("/review ".Length).Trim();

                            // send message without the command part
                            await targetChannel.SendMessageAsync($"Review is in progress for: {messageContentWithoutCommand}");

                        }
                        else
                        {
                            // if the channel id is incorrect
                            Console.WriteLine("Target channel not found.");
                        }
                    }
                }
                else if (message.Content.StartsWith("/choose"))
                {
                    // trim off the start of the command
                    string choicesString = message.Content.Substring("/choose ".Length).Trim();

                    if (!string.IsNullOrEmpty(choicesString))
                    {
                        // split by comma
                        var choices = choicesString.Split(',');

                        // remove extra space
                        choices = choices.Select(choice => choice.Trim()).ToArray();

                        // randomly pick choice
                        var random = new Random();
                        var chosenOption = choices[random.Next(choices.Length)];

                        // send chosen option
                        await textChannel.SendMessageAsync($"I have chosen: {chosenOption}");
                    }
                    else
                    {
                        await textChannel.SendMessageAsync("Please provide a list of choices.");
                    }
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

            // make sure message is correct and not from a dm/somewhere the bot cannot access
            if (message.Channel is SocketTextChannel textChannel)
            {
                ulong sourceChannelId = 1335950203769065492;  // channel id of application channel
                if (textChannel.Id == sourceChannelId)
                {
                    ulong targetChannelId = 1337883290463371304;  // channel id of mod channel

                    // double check channel id
                    var targetChannel = textChannel.Guild.GetTextChannel(targetChannelId);

                    if (targetChannel != null)
                    {
                        // send message
                        await targetChannel.SendMessageAsync($"New Application from: {message.Author.Username}\nApplication Contents: {message.Content}");
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
                Console.WriteLine("Message error regarding receiving app. Please try again.");
            }

        }
    }
}
