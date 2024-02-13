
using Microsoft.EntityFrameworkCore;
using ZenithBeepData.Models;


namespace ZenithBeepData.Context
{
    public class BeepDbContext : DbContext
    {
        public BeepDbContext(DbContextOptions options) : base(options)
        {

        }

        
        public DbSet<ModelGuild> Guilds { get; set; }
        public DbSet<ModelRoles> Roles { get; set; }
        public DbSet<ModelRooms> Rooms { get; set; }
        public DbSet<ModelRoomsLobby> RoomsLobbys { get; set; }
        public DbSet<ModelTempRoom> TempRooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ModelTempRoom>()
                .HasKey(e => e.userId);
                
        }
    }
}
