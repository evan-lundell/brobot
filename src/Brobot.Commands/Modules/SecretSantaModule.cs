using Brobot.Core.Exceptions;
using Brobot.Core.Models;
using Brobot.Core.Services;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Brobot.Commands.Modules
{
    public class SecretSantaModule : ModuleBase<SocketCommandContext>
    {
        public IBrobotService BrobotService { get; set; }
        public Random Random { get; set; }

        private Dictionary<string, Func<string[], Task>> _subcommands;

        public SecretSantaModule()
        {
            _subcommands = new Dictionary<string, Func<string[], Task>>();
            _subcommands.Add("creategroup", CreateGroup);
            _subcommands.Add("addmember", AddMember);
            _subcommands.Add("createevent", CreateEvent);
        }

        [Command("secretsanta")]
        public async Task SecretSanta(string subcommand, params string[] args)
        {
            try
            {
                var discordUser = await BrobotService.GetDiscordUser(Context.Message.Author.Id);
                if (!discordUser.BrobotAdmin)
                {
                    return;
                }
                var subcommandLower = subcommand.ToLower();
                if (!_subcommands.ContainsKey(subcommandLower))
                {
                    await ReplyAsync($"{subcommand} is not a valid subcommand");
                    return;
                }

                await _subcommands[subcommandLower](args);
            }
            catch (BrobotServiceException bsEx)
            {
                if (!string.IsNullOrWhiteSpace(bsEx.MessageFromServer))
                {
                    await ReplyAsync(bsEx.MessageFromServer);
                }
                else
                {
                    await ReplyAsync(bsEx.Message);
                }
            }
            catch (Exception ex)
            {
                await ReplyAsync($"Failed to execute secret santa command: {ex.Message}");
            }
        }

        private async Task CreateGroup(params string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                await ReplyAsync("Invalid arguments. Supply a group name and optional check past year");
                return;
            }

            var checkPastYear = false;
            if (args.Length == 2)
            {
                if (!bool.TryParse(args[1], out checkPastYear))
                {
                    await ReplyAsync($"Unable to parse {args[1]} for use past year value. Use true or false");
                }
            }

            var secretSantaGroup = new SecretSantaGroup
            {
                Name = args[0],
                CheckPastYearPairings = checkPastYear
            };

            var newSecretSantaGroup = await BrobotService.CreateSecretSantaGroup(secretSantaGroup);
            await ReplyAsync($"Created group {newSecretSantaGroup.Name} with an ID of {newSecretSantaGroup.SecretSantaGroupId}");
        }

        private async Task AddMember(params string[] args)
        {
            if (args.Length != 2)
            {
                await ReplyAsync("Provide 2 arguments, group id and discord user id");
                return;
            }

            if (!int.TryParse(args[0], out int groupId))
            {
                await ReplyAsync($"Unable to parse {args[0]} as group id");
                return;
            }

            if (!ulong.TryParse(args[1], out ulong discordUserId))
            {
                await ReplyAsync($"Unable to parse {args[1]} as discord user id");
            }

            await BrobotService.AddDiscordUserToSecretSantaGroup(groupId, discordUserId);
            await ReplyAsync("Added");
        }

        private async Task CreateEvent(string[] args)
        {
            if (args.Length != 1)
            {
                await ReplyAsync("Provide 1 argument, the group to create the event for");
                return;
            }

            if (!int.TryParse(args[0], out int groupId))
            {
                await ReplyAsync($"Unable to parse {args[0]} as group id");
                return;
            }

            var newEvent = await BrobotService.CreateSecretSantaEvent(groupId, DateTime.Now.Year, Context.User.Id);
            foreach (var pairing in newEvent.SecretSantaPairings)
            {
                var user = Context.Client.GetUser(pairing.Giver.DiscordUserId);
                var dmChannel = await user.GetOrCreateDMChannelAsync();
                await dmChannel.SendMessageAsync($":santa: You have {pairing.Recipient.Username}");
            }

            await ReplyAsync("Pairings sent!");
        }
    }
}
