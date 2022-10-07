using Brobot.Commands.Services;
using Brobot.Core.Models;
using Brobot.Core.Services;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Brobot.Commands.Modules
{
    public class BrobotModule : ModuleBase<SocketCommandContext>
    {
        public IBrobotService BrobotService { get; set; }
        public Random Random { get; set; }
        public IRandomFactService RandomFactService { get; set; }
        public IGiphyService GiphyService { get; set; }
        public IDefinitionService DefinitionService { get; set; }

        [Command("game")]
        [Summary("Picks a random game from the given list. Ex: !game \"Rocket League\" \"Overwatch\" \"Dauntless\"")]
        public async Task Game(params string[] games)
        {
            await ReplyAsync($"Let's play {games[Random.Next(0, games.Length)]}!");
        }

        [Command("dice")]
        [Alias("roll")]
        [Summary("Rolls dice based on the given parameter. Ex: !dice 2d8. !roll #d# is also valid")]
        public async Task Dice(string dice)
        {
            var nums = dice.Split('d');
            if (nums.Length != 2 || !int.TryParse(nums[0], out int numOfDice) || !int.TryParse(nums[1], out int diceValue))
            {
                await ReplyAsync("Format has to be in NdN format (ex: 1d20).");
                return;
            }

            if (numOfDice > 25)
            {
                await ReplyAsync("Why do you need to roll that many dice? :thinking:");
                return;
            }

            var results = new string[numOfDice];
            for (int i = 0; i < numOfDice; i++)
            {
                results[i] = Random.Next(1, diceValue + 1).ToString();
            }
            await ReplyAsync(string.Join(", ", results));
        }

        [Command("info")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        [Summary("Gets server info")]
        public async Task Info()
        {
            await ReplyAsync($"Server: {Context.Guild.Id}\nChannel: {Context.Channel.Id}");
        }

        [Command("teams")]
        [Summary("Generates two teams based on the list of provided players. Ex: !teams Player1 Player2 Player3 Player4")]
        public async Task Teams(params string[] args)
        {
            var players = new List<string>(args);
            var team1 = new List<string>();
            var team1Length = args.Length % 2 == 0 ? args.Length / 2 : (args.Length / 2) + 1;

            for (int i = 0; i < team1Length; i++)
            {
                var player = players[Random.Next(0, players.Count)];
                team1.Add(player);
                players.Remove(player);
            }

            await ReplyAsync($"Team 1: {string.Join(", ", team1)}\nTeam 2: {string.Join(", ", players)}");
        }

        [Command("pika")]
        [Summary("Shocked Pikachu")]
        public async Task Pika()
        {
            await Context.Channel.SendFileAsync("./Images/pika.jpeg");
        }

        [Command("doh")]
        [Summary("Doh!")]
        public async Task Doh()
        {
            await Context.Channel.SendFileAsync("./Images/doh.png");
        }

        [Command("sarcasm")]
        [Summary("Post the provided text in SaRcAsM fOnT. Ex: !sarcasm This is a sarcastic statement")]
        public async Task Sarcasm([Remainder]string text)
        {
            var response = new StringBuilder();
            bool capitalize = true;
            foreach (var c in text)
            {
                if (char.IsLetter(c))
                {
                    response.Append(capitalize ? char.ToUpper(c) : char.ToLower(c));
                    capitalize = !capitalize;
                }
                else
                {
                    response.Append(c);
                }
            }

            await ReplyAsync(response.ToString());
        }

        [Command("reminder")]
        [Summary("Saves a reminder for this channel. Requires two parameters, a date/time (yyyy-MM-dd HH:mm) and a message. Ex: !reminder \"2020-01-01 18:00\" \"This is a reminder\"")]
        public async Task Reminder(string reminderDateTime, string reminderMessage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(reminderDateTime) || string.IsNullOrWhiteSpace(reminderMessage))
                {
                    await ReplyAsync("Invalid arguments. Please supply two arguments: a date in yyyy-MM-dd HH:mm format and a reminder message");
                    return;
                }

                if (!DateTime.TryParse(reminderDateTime, out DateTime dateTimeResult))
                {
                    await ReplyAsync($"{reminderDateTime} is not in the format. Please use yyyy-MM-dd HH:mm format (ex: 2020-01-01 18:00)");
                    return;
                }

                var reminder = new Reminder
                {
                    ChannelId = Context.Channel.Id,
                    OwnerId = Context.Message.Author.Id,
                    Message = reminderMessage,
                    ReminderDateUtc = dateTimeResult
                };
                await BrobotService.PostReminder(reminder);
                await ReplyAsync("Reminder has been set");
            }
            catch (Exception)
            {
                await ReplyAsync("Failed to set reminder message");
            }
        }

        [Command("fact")]
        [Summary("Gets a random fact")]
        public async Task Fact()
        {
            try
            {
                var fact = await RandomFactService.GetRandomFactAsync();
                await ReplyAsync(fact.Text);
            }
            catch (Exception)
            {
                await ReplyAsync("Failed to get random fact");
            }
        }

        [Command("gif")]
        [Summary("Posts a random gif")]
        public async Task Gif([Remainder]string tag = null)
        {
            try
            {
                var giphyResponse = await GiphyService.GetRandomGif(tag);
                await ReplyAsync(giphyResponse.Data.Url);
            }
            catch (Exception)
            {
                await ReplyAsync("Failed to get random gif");
            }
        }

        [Command("hotop")]
        [Summary("Gets the scores for the active hot ops")]
        public async Task HotOp()
        {
            try
            {
                var hotOpScoreboards = await BrobotService.GetHotOpScoreboards(Context.Channel.Id);

                if (hotOpScoreboards.Count() == 0)
                {
                    await ReplyAsync("No current hot ops");
                    return;
                }

                foreach (var hotOpScoreboard in hotOpScoreboards)
                {
                    var builder = new EmbedBuilder
                    {
                        Color = new Color(114, 137, 218),
                        Description = $"Operation Hot {hotOpScoreboard.HotOpOwnerName}"
                    };

                    foreach (var score in hotOpScoreboard.Scores.OrderByDescending(s => s.Score))
                    {
                        builder.AddField(x =>
                        {
                            x.Name = score.DiscordUserName;
                            x.Value = score.Score;
                            x.IsInline = false;
                        });
                    }

                    await ReplyAsync("", false, builder.Build());
                }
            }
            catch (Exception)
            {
                await ReplyAsync("Failed to get hot op scores");
            }
        }

        [Command("define")]
        [Summary("Gets the definition of a word")]
        public async Task Define(string word)
        {
            if (word.Split(" ").Length > 1)
            {
                await ReplyAsync("Only one word allowed");
                return;
            }

            var definition = await DefinitionService.GetDefinitions(word);
            await ReplyAsync(definition);
        }
    }
}
