
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
     

        public void ApplyMigrations()
        {
            if (Database.GetPendingMigrations().Any()) {
                Database.Migrate();
                Console.WriteLine("Migrations applied successfully");
            }
        }

        public void CreateMigration(string migrationName)
        {
            Database.EnsureCreated();

            try
            {
                Database.ExecuteSqlRaw($"CREATE TABLE __EFMigrationsHistory (MigrationId varchar(150) NOT NULL, ProductVersion varchar(32) NOT NULL);");
                Database.ExecuteSqlRaw($"INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('{DateTime.Now.Ticks}_InitialMigration', '5.0.0')");
                Console.WriteLine("Migration created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating migration: {ex.Message}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ModelTempRoom>()
                .HasKey(e => e.userId);
                
        }
    }
}
