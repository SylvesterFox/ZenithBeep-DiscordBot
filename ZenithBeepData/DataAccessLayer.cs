
using Microsoft.EntityFrameworkCore;

using ZenithBeepData.Context;
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

        public string GetPrefix(ulong Id)
        {
            using var context = _contextFactory.CreateDbContext();
            var guild = context.Guilds
              .FirstOrDefault(x => x.guildId == Id);

            if (guild == null)
            {
                guild = context.Add(new ModelGuild { guildId = Id }).Entity;
                context.SaveChanges();
            }

            return guild.Prefix;
        }

        public string GetLanguage(ulong IdGuild)
        {
            using var context = _contextFactory.CreateDbContext();
            var guild = context.Guilds
               .Find(IdGuild);

            if (guild == null)
            {
                guild = context.Add(new ModelGuild { guildId = IdGuild }).Entity;
                context.SaveChanges();
            }
            return guild.Lang;
                
        }

/*        public int GetPrimeryId(ulong IdGuild)
        {
            using var context = _contextFactory.CreateDbContext();
            var guild = context.Guilds.Find(IdGuild);

            if (guild == null)
            {
                guild = context.Add(new ModelGuild { Id = IdGuild }).Entity;
                context.SaveChanges();
            }
            return guild.Id;
        }
*/
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
                .FindAsync(IdGuild);

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

        public async Task DeleteGuild(ulong IdGuild)
        {
            using var context = _contextFactory.CreateDbContext();
            var guild = await context.Guilds
                .FindAsync(IdGuild);

            if (guild == null)
                return;

            context.Remove(guild);
            await context.SaveChangesAsync();
        }

    }
}
