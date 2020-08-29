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

            var description = new StringBuilder();
            foreach (var command in brobotModule.Commands)
            {
                var result = await command.CheckPreconditionsAsync(Context);
                if (result.IsSuccess)
                {
                    description.AppendLine($"!{command.Aliases.First()}");
                }
            }

            if (description.Length > 0)
            {
                builder.AddField(x =>
                {
                    x.Name = brobotModule.Name;
                    x.Value = description.ToString();
                    x.IsInline = false;
                });
            }

            await ReplyAsync("", false, builder.Build());
        }
    }
}
