
using Microsoft.EntityFrameworkCore;

using ZenithBeepData.Context;
using ZenithBeepData.ExceptionData;
using ZenithBeepData.Models;

namespace ZenithBeepData
{
    public class DataAccessLayer
    {
        private readonly IDbContextFactory<BeepDbContext> _contextFactory;

        public DataAccessLayer(IDbContextFactory<BeepDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task CreateGuild (ulong IdGuild)
        {
            using var context = _contextFactory.CreateDbContext();
            if (context.Guilds.Any(x => x.guildId == IdGuild))
                return;

            context.Add(new ModelGuild { guildId = IdGuild });
            await context.SaveChangesAsync();
        }

        public string GetPrefix(ulong IdGuild)
        {
            using var context = _contextFactory.CreateDbContext();
            var guild = context.Guilds
              .Where(x => x.guildId == IdGuild).FirstOrDefault();

            if (guild == null)
            {
                guild = context.Add(new ModelGuild { guildId = IdGuild }).Entity;
                context.SaveChanges();
            }

            return guild.Prefix;
        }

        public string GetLanguage(ulong IdGuild)
        {
            using var context = _contextFactory.CreateDbContext();
            var guild = context.Guilds
                .Where(x => x.guildId == IdGuild).FirstOrDefault();
               

            if (guild == null)
            {
                guild = context.Add(new ModelGuild { guildId = IdGuild }).Entity;
                context.SaveChanges();
            }
            return guild.Lang;
                
        }

        public int GetGuildPrimeryId(ulong IdGuild)
        {
            using var context = _contextFactory.CreateDbContext();
            var guild = context.Guilds.Where(x => x.guildId == IdGuild).FirstOrDefault();

            if (guild == null)
            {
                guild = context.Add(new ModelGuild { guildId = IdGuild }).Entity;
                context.SaveChanges();
            }
            return guild.Id;
        }

        public async Task SetPrefix(ulong guildId, string prefix)
        {
            using var context = _contextFactory.CreateDbContext();
            var guild = await context.Guilds
                .Where(x => x.guildId == guildId).FirstOrDefaultAsync();
                

            if (guild != null)
            {
                guild.Prefix = prefix;
            }
            else
            {
                context.Add(new ModelGuild { guildId = guildId, Prefix = prefix });

            }

            await context.SaveChangesAsync();

        }

        public async Task SetLanguage(ulong IdGuild, string lang)
        {
            using var context = _contextFactory.CreateDbContext();
            var guild = await context.Guilds
                .Where(x => x.guildId == IdGuild).FirstOrDefaultAsync();

            if (guild != null)
            {
                guild.Lang = lang;
            } 
            else
            {
                context.Add(new ModelGuild { guildId = IdGuild, Lang = lang });
            }
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Roles insert database
        /// </summary>
        /// <param name="IdGuild"></param>
        /// <param name="messageId"></param>
        /// <param name="roleId"></param>
        /// <param name="channelId"></param>
        /// <param name="Emoji"></param>
        /// <returns></returns>
        /// <exception cref="DataObjectExists"></exception>
        public async Task SetRolesAutoMod(ulong IdGuild, ulong messageId, ulong roleId, ulong channelId, string Emoji)
        {
            using var context = _contextFactory.CreateDbContext();
            var guild = await context.Guilds
                    .Where(x => x.guildId == IdGuild).FirstOrDefaultAsync();
            
            
            if (guild == null)
            {
                context.Add(new ModelGuild { guildId = IdGuild });
                await context.SaveChangesAsync();
                guild = await context.Guilds
                    .Where(x => x.guildId == IdGuild).FirstOrDefaultAsync();
            }

            var roles = await context.Roles
                .Where(x => x.roleId == roleId)
                .Where(x => x.Id == guild.Id)
                .Where(x => x.messageId == messageId)
                .FirstOrDefaultAsync();

            if (roles == null)
            {
                context.Add(new ModelRoles { messageId = messageId, channelId = channelId, GuildId = guild.Id, roleId = roleId, setEmoji = Emoji });
                await context.SaveChangesAsync();
            } else
            {
                throw new DataObjectExists("This object already exists!");
            }
        }

        /// <summary>
        /// delete role
        /// </summary>
        /// <param name="guildId"></param>
        
        /// <returns></returns>
        public async Task DeleteRolesAutoMod(ulong guildId, int primirykeyId)
        {
            using var context = _contextFactory.CreateDbContext();

            var guild = await context.Guilds
                    .Where(x => x.guildId == guildId).FirstOrDefaultAsync();


            if (guild == null)
            {
                return;
            }
           
            var roles = await context.Roles
                .Where(x => x.GuildId == guild.Id)
                .Where(x => x.Id == primirykeyId)
                .FirstOrDefaultAsync();

            if (roles == null)
            {
                return;
            }
            context.Remove(roles);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Get role from database
        /// </summary>
        /// <param name="guildId"></param>
        /// <param name="emoji"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public async Task<ModelRoles> GetRoleAutoMod(ulong guildId, ulong messageId, string emoji)
        {

            using var context = _contextFactory.CreateDbContext();

            var guild = await context.Guilds
                    .Where(x => x.guildId == guildId).FirstOrDefaultAsync();


            if (guild == null)
                return null;

            var roles = context.Roles
                .Where(x => x.GuildId == guild.Id)
                .Where(x => x.setEmoji == emoji)
                .Where(x => x.messageId == messageId)
                .FirstOrDefaultAsync();

            if (roles == null)
                return null;

            return await roles;
        }

        public async Task<ModelRoles> GetKeyRole(ulong guildId, int key)
        {
            using var context = _contextFactory.CreateDbContext();

            var guild = await context.Guilds
                    .Where(x => x.guildId == guildId).FirstOrDefaultAsync();


            if (guild == null)
                return null;

            var roles = context.Roles
                .Where(x => x.GuildId == guild.Id)
                .Where(x => x.Id == key)
                .FirstOrDefaultAsync();

            if (roles == null)
                return null;


            return await roles;

        }

        public async Task<List<ModelRoles>> GetAllRoloesOfMessage(ulong guildId, ulong messageId) 
        {
            using var context = _contextFactory.CreateDbContext();

            var guild = await context.Guilds
                    .Where(x => x.guildId == guildId).FirstOrDefaultAsync();


            if (guild == null)
                return null;


            var roles = await context.Roles
                .Where(x => x.GuildId == guild.Id)
                .Where(x => x.messageId == messageId)
                .ToListAsync();

            if (roles.Count == 0)
            {
                throw new NotFoundObjectData("No roles found for the linked message");
            }

            return roles;
        }


        public async Task DeleteGuild(ulong IdGuild)
        {
            using var context = _contextFactory.CreateDbContext();
            var guild = await context.Guilds
                .Where(x => x.guildId == IdGuild).FirstOrDefaultAsync();

            if (guild == null)
                return;

            context.Remove(guild);
            await context.SaveChangesAsync();
        }

    }
}
