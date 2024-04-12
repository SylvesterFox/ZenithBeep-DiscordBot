
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
              .Where(x => x.Id == IdGuild).FirstOrDefault();


            context.Add(new ModelRoomsLobby { GuildId = guild.Id, lobby_id = Idchannel });
            await context.SaveChangesAsync();
        }

        public async Task<ModelRoomsLobby?> GetLobby(ulong Id)
        {
            using var context = _contextFactory.CreateDbContext();

            var lobby = context.RoomsLobbys.Where(x => x.GuildId == Id);

            if (await lobby.AnyAsync() == false)
            {
                return null;
            }

            return await lobby.FirstAsync();
        }

        public async Task DeleteLobby(ulong channelId)
        {
            using var context = _contextFactory.CreateDbContext();
            var lobby = context.RoomsLobbys.Where(x => x.lobby_id == channelId);

            if (await lobby.AnyAsync() == false)
            {
                throw new NotFoundObjectData("Lobby does not exist");
            }

            context.Remove(await lobby.FirstAsync());
            await context.SaveChangesAsync();
        }

        public async Task<ModelRooms> CreateOrGetRoomSettings(ulong ownerId, string? name = null, int limit = 0)
        {
            using var context = _contextFactory.CreateDbContext();
            var room = await context.Rooms.Where(x => x.OwnerChannelId == ownerId).FirstOrDefaultAsync();

            if (room == null)
            {
                room = context.Add(new ModelRooms { Name = name, limit = limit, OwnerChannelId = ownerId }).Entity;
                await context.SaveChangesAsync();
            }

            return room;
        }

        public async Task SetVoicelimit(int linit, ulong owner)
        {
            using var context = _contextFactory.CreateDbContext();
            var room = await context.Rooms.Where(x => x.OwnerChannelId == owner).FirstOrDefaultAsync();
            if (room != null)
            {
                room.limit = linit;
                await context.SaveChangesAsync();
            }

            return;
        }

        public async Task SetVoiceName(string voiceName, ulong owner)
        {
            using var context = _contextFactory.CreateDbContext();
            ModelRooms rooms;
            rooms = await context.Rooms.Where(x => x.OwnerChannelId == owner).FirstAsync();
            if (rooms != null)
            {
                rooms.Name = voiceName;
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
