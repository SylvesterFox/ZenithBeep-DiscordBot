using Discord;
using Discord.WebSocket;
using ZenithBeepData;


namespace ZenithBeep.Handlers
{

    public class HanderRoles
    {
        private readonly DiscordSocketClient _clinet;
        public readonly DataAccessLayer DataAccessLayer;

        public HanderRoles (DiscordSocketClient clinet, IServiceProvider service, DataAccessLayer dataAccessLayer)
        {
            _clinet = clinet;
            DataAccessLayer = dataAccessLayer;
        }

        public async Task InitializeAsync()
        {
            _clinet.ReactionAdded += OnReactionAdded;
            _clinet.ReactionRemoved += OnReactionRemoved;
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cacheable1, Cacheable<IMessageChannel, ulong> cacheable2, SocketReaction reaction)
        {

            if (reaction.User.Value.IsBot)
            {
                return;
            }

            var guild = ((SocketGuildChannel)reaction.Channel).Guild;
            var _emoji = reaction.Emote.ToString();

            Console.WriteLine(_emoji);
            var roledb = await DataAccessLayer.GetRoleAutoMod(guild.Id, reaction.MessageId, _emoji);

            if (roledb is null)
            {
                return;
            }

            var role = guild.GetRole(roledb.roleId);
            await ((SocketGuildUser)reaction.User.Value).AddRoleAsync(role);
        }



        private async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> cacheable1, Cacheable<IMessageChannel, ulong> cacheable2, SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot)
            {
                return;
            }

            var guild = ((SocketGuildChannel)reaction.Channel).Guild;
            var _emoji = reaction.Emote.ToString();

            Console.WriteLine(_emoji);
            var roledb = await DataAccessLayer.GetRoleAutoMod(guild.Id, reaction.MessageId, _emoji);

            if (roledb is null)
            {
                return;
            }

            var role = guild.GetRole(roledb.roleId);
            await ((SocketGuildUser)reaction.User.Value).RemoveRoleAsync(role);

        }

    }
}
