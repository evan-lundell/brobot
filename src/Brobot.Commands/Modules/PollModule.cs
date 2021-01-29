using Brobot.Commands.Models;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brobot.Commands.Modules
{
    public class PollModule : ModuleBase<SocketCommandContext>
    {
        [Command("poll")]
        [Summary("Creates a poll. Ex: !poll \"Who is your favorite super hero?\" \"Superman\" \"Spider-Man\" \"Wonder Woman\"")]
        public async Task Poll(params string[] options)
        {
            if (options.Length == 0)
            {
                await ReplyAsync("Provide a question and a list of options");
                return;
            }

            if (options.Length < 3)
            {
                await ReplyAsync("Provide a question and at least two options");
                return;
            }

            if (options.Length > 11)
            {
                await ReplyAsync("Only 10 options are allowed");
                return;
            }

            var question = options[0];
            var answers = options.Skip(1).Take(options.Length - 1).ToArray();

            var response = new StringBuilder($"**{question}**\n\n");
            for (int i = 0; i < answers.Length; i++)
            {
                response.AppendLine($"{PollEmoji.DiscordCode[i + 1]} {answers[i]}");
            }

            var message = await ReplyAsync(response.ToString());
            var reactions = new IEmote[answers.Length];

            for (int i = 0; i < answers.Length; i++)
            {
                reactions[i] = new Emoji(PollEmoji.Unicode[i + 1]);
            }

            await message.AddReactionsAsync(reactions);
        }
    }
}
