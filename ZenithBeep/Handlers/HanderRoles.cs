using Discord;
using Discord.WebSocket;


namespace ZenithBeep.Handlers
{

    public class HanderRoles
    {
        private readonly DiscordSocketClient _clinet;
        private readonly IServiceProvider _service;

        public HanderRoles (DiscordSocketClient clinet, IServiceProvider service)
        {
            _clinet = clinet;
            _service = service;
        }

        public async Task InitializeAsync()
        {
            _clinet.ReactionAdded += OnReactionAdded;
            _clinet.ReactionRemoved += OnReactionRemoved;
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cacheable1, Cacheable<IMessageChannel, ulong> cacheable2, SocketReaction reaction)
        {
/*
            if (reaction.User.Value.IsBot)
            {
                return;
            }

            var get_role = new ModelRoles
            {
                setEmoji = reaction.Emote.ToString(),
                messageId = (long)reaction.MessageId
            };

            ModelRoles roledb = DatabasePost.GetRole<ModelRoles>(get_role);

            if (roledb is null)
            {
                return;
            }

            Console.WriteLine(roledb.roleName);

            var role = ((SocketGuildChannel)reaction.Channel).Guild.GetRole((ulong)roledb.roleId);
            await ((SocketGuildUser)reaction.User.Value).AddRoleAsync(role);*/
        }



        private async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> cacheable1, Cacheable<IMessageChannel, ulong> cacheable2, SocketReaction reaction)
        {
          /*  if (reaction.User.Value.IsBot)
            {
                return;
            }


            var get_role = new ModelRoles
            {
                setEmoji = reaction.Emote.ToString(),
                messageId = (long)reaction.MessageId
            };


            ModelRoles roledb = DatabasePost.GetRole<ModelRoles>(get_role);

            if (roledb is null)
            {
                return;
            }

            var role = ((SocketGuildChannel)reaction.Channel).Guild.GetRole((ulong)roledb.roleId);
            await ((SocketGuildUser)reaction.User.Value).RemoveRoleAsync(role);*/

        }

    }
}
