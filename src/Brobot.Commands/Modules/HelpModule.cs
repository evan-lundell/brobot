using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brobot.Commands.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;

        public HelpModule(CommandService service)
        {
            _service = service;
        }

        [Command("help")]
        public async Task HelpAsync()
        {
            var builder = new EmbedBuilder
            {
                Color = new Color(114, 137, 218),
                Description = "Commands"
            };

            var brobotModule = _service.Modules.FirstOrDefault(m => m.Name.Equals("brobotmodule", StringComparison.OrdinalIgnoreCase));
            if (brobotModule == null)
            {
                return;
            }

            foreach (var command in brobotModule.Commands)
            {
                var result = await command.CheckPreconditionsAsync(Context);
                if (result.IsSuccess)
                {
                    builder.AddField(x =>
                    {
                        x.Name = $"!{command.Aliases.First()}";
                        string value = "";
                        if (command.Parameters.Count > 0)
                        {
                            value += $"Parameters: {string.Join(", ", command.Parameters.Select(p => p.Name))}\n";
                        }
                        value += $"Summary: {command.Summary}";
                        x.Value = value;
                        x.IsInline = false;
                    });
                }
            }

            await ReplyAsync("", false, builder.Build());
        }

        [Command("help")]
        public async Task HelpAsync(string command)
        {
            var result = _service.Search(Context, command);

            if (!result.IsSuccess)
            {
                await ReplyAsync($"{command} does not exist. Type !help for a full list of commands.");
                return;
            }

            var builder = new EmbedBuilder
            {
                Color = new Color(114, 137, 218)
            };

            foreach (var match in result.Commands)
            {
                var cmd = match.Command;
                builder.AddField(x =>
                {
                    x.Name = $"!{cmd.Aliases.First()}";
                    string value = "";
                    if (cmd.Parameters.Count > 0)
                    {
                        value += $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n";
                    }
                    value += $"Summary: {cmd.Summary}";
                    x.Value = value;
                    x.IsInline = false;
                });
            }

            await ReplyAsync("", false, builder.Build());
        }
    }
}
