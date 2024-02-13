
using Microsoft.EntityFrameworkCore;
using ZenithBeepData.Context;
using ZenithBeepData.ExceptionData;
using ZenithBeepData.Models;

namespace ZenithBeepData
{
    public class DataRooms
    {
        private readonly IDbContextFactory<BeepDbContext> _contextFactory;

        public DataRooms(IDbContextFactory<BeepDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task CreateLobby(ulong IdGuild, ulong Idchannel)
        {
            using var context = _contextFactory.CreateDbContext();
            var guild = context.Guilds
              .Where(x => x.guildId == IdGuild).FirstOrDefault();

            if (guild == null)
            {
                guild = context.Add(new ModelGuild { guildId = IdGuild }).Entity;
                await context.SaveChangesAsync();
            }

            context.Add(new ModelRoomsLobby { GuildId = guild.Id, lobby_id = Idchannel });
            await context.SaveChangesAsync();
        }

        public async Task<ModelRoomsLobby> GetLobby(ulong IdGuild)
        {
            using var context = _contextFactory.CreateDbContext();
            var guild = context.Guilds
              .Where(x => x.guildId == IdGuild).FirstOrDefault();

            if (guild == null)
            {
                guild = context.Add(new ModelGuild { guildId = IdGuild }).Entity;
                await context.SaveChangesAsync();
            }

            var lobby = context.RoomsLobbys.Where(x => x.GuildId == guild.Id).FirstOrDefault();

            if (lobby == null)
            {
                return null;
            }

            return lobby;
        }

        public async Task DeleteLobby(int key)
        {
            using var context = _contextFactory.CreateDbContext();
            var lobby = context.RoomsLobbys.Where(x => x.Id == key).FirstOrDefault();

            if (lobby == null)
            {
                throw new NotFoundObjectData("Lobby does not exist");
            }
            context.Remove(lobby);
            await context.SaveChangesAsync();
        }

        public async Task<ModelRooms> GetRoomSettings(ulong ownerId, string? name = null, int limit = 0)
        {
            using var context = _contextFactory.CreateDbContext();
            var room = await context.Rooms.Where(x => x.owner_channel == ownerId).FirstOrDefaultAsync();

            if (room == null)
            {
                room = context.Add(new ModelRooms { channel_name = name, limit = limit, owner_channel = ownerId }).Entity;
                await context.SaveChangesAsync();
            }

            return room;
        }

        public async Task SetVoicelimit(int linit, ulong owner)
        {
            using var context = _contextFactory.CreateDbContext();
            var room = await context.Rooms.Where(x => x.owner_channel == owner).FirstOrDefaultAsync();
            if (room != null)
            {
                room.limit = linit;
                await context.SaveChangesAsync();
            }

            return;
        }

        public async Task CreateTempRoom(ulong IdChannel, ulong userId)
        {
            using var context = _contextFactory.CreateDbContext();
            var temproom = context.TempRooms.Where(x => x.channelRoomId == IdChannel).Where(x => x.userId == userId).FirstOrDefault();

            if (temproom != null)
            {
                return;
            }

            context.Add(new ModelTempRoom { channelRoomId = IdChannel, userId = userId});
            await context.SaveChangesAsync();
        }

        public async Task<ModelTempRoom> GetTempRoom(ulong IdChannel)
        {
            using var context = _contextFactory.CreateDbContext();
            var temproom = await context.TempRooms.Where(x => x.channelRoomId == IdChannel).FirstOrDefaultAsync();

            return temproom;
        }

        public async Task DeleteTempRoom(ulong idchannel) 
        {
            using var context = _contextFactory.CreateDbContext();
            var temproom = context.TempRooms.Where(x => x.channelRoomId == idchannel).FirstOrDefault();

            if (temproom == null)
            {
                return;
            }

            context.Remove(temproom);
            await context.SaveChangesAsync();
        }
    }
}
