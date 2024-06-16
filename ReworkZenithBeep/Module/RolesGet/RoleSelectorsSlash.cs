

using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ReworkZenithBeep.Data;
using ReworkZenithBeep.Settings;

namespace ReworkZenithBeep.Module.RolesGet
{
    public class RoleSelectorsSlash : ApplicationCommandModule
    {
        private readonly RoleSelectors _roles;
        public RoleSelectorsSlash(DataBot contextDB)
        {
            _roles = new RoleSelectors(contextDB);
        }

        [SlashCommand("roleselector-create", "Create role selector")]
        public async Task CommandCreateRoleSelect(
            InteractionContext context,
            [Option("Roles", "Select role")] DiscordRole discordRole, 
            [Option("MesaageId", "Message id")] string messageId, 
            [Option("Emoji", "Your emoji")] string emoji,
            [Option("Channel", "Select channel")] DiscordChannel? channel = null)
        {
            var id = Convert.ToUInt64(messageId);
            await _roles.CreateRolesCommand(context, channel, discordRole, id, emoji);
        }
        
    }
}
