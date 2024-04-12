
using Microsoft.EntityFrameworkCore;
using ZenithBeepData.Models;


namespace ZenithBeepData.Context
{
    public class BeepDbContext : DbContext
    {
        
        public BeepDbContext(DbContextOptions options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
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

        public override int SaveChanges()
        {
            updateChangeTrackerDates();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            updateChangeTrackerDates();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellation = default)
        {
            updateChangeTrackerDates();
            return base.SaveChangesAsync(cancellation);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            updateChangeTrackerDates();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }


        private void updateChangeTrackerDates()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => (e.Entity is BaseDbEntity) && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                    ((BaseDbEntity)entry.Entity).CreatedAt = DateTime.Now;
                else if (entry.State == EntityState.Modified)
                    ((BaseDbEntity)entry.Entity).UpdatedAt = DateTime.Now;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ModelTempRoom>()
                .HasKey(e => e.userId);

            modelBuilder.Entity<ModelRoomsLobby>()
                .HasKey(e => e.lobby_id);

            modelBuilder.Entity<ModelRooms>()
                .HasKey(e => e.OwnerChannelId);

            
                
        }
    }
}
