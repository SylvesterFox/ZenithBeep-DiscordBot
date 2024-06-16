
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using ReworkZenithBeep.Data.Models.items;

namespace ReworkZenithBeep.Data
{
    public class DataBot
    {
        public readonly IDbContextFactory<BotContext> _contextFactory;
        public DataBot(IDbContextFactory<BotContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        /// <summary>
        /// Creates and receives a guild from the base
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        public async Task<ItemGuild> GetOrCreateGuild(DiscordGuild guild)
        {
            ItemGuild itemGuild;
            using var context = _contextFactory.CreateDbContext();
            var query = context.Guilds.Where(x => x.Id == guild.Id);

            if (await query.AnyAsync() == false) 
            {
                itemGuild = new ItemGuild()
                {
                    Id = guild.Id,
                    Name = guild.Name,
                };
                context.Add(itemGuild);
                await context.SaveChangesAsync();
            } else itemGuild = await query.FirstAsync();

            return itemGuild;
        }
        /// <summary>
        /// Prefix update request
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public async Task UpdatePrefix(DiscordGuild guild, string prefix)
        {
            using var context = _contextFactory.CreateDbContext();
            var guildContext = await context.Guilds.Where(x => x.Id == guild.Id).FirstOrDefaultAsync();
            
            
            if (guildContext != null)
            {
                guildContext.Prefix = prefix;
            } else
            {
                context.Add(new ItemGuild {  Id = guild.Id, Name = guild.Name, Prefix = prefix });
            }

            await context.SaveChangesAsync();
        }

        public async Task<bool> CreateRolesSelector(DiscordGuild guild, ulong messageId, ulong roleId, ulong channelId, string Emoji)
        {
            using var context = _contextFactory.CreateDbContext();
            ItemGuild contextGuild = await GetOrCreateGuild(guild);
            ItemRolesSelector itemRolesSelector;
            var query = context.Roles.Where(x => x.roleId == roleId)
                .Where(x => x.Id == contextGuild.Id)
                .Where(x => x.messageId == messageId);
                

            if (await query.AnyAsync() == false)
            {
                itemRolesSelector = new ItemRolesSelector
                {
                    Id = contextGuild.Id,
                    messageId = messageId,
                    channelId = channelId,
                    roleId = roleId,
                    emojiButton = Emoji,
                };
                context.Add(itemRolesSelector);
                await context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }

        }

    }
}
