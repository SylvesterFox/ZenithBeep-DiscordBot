
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
        public DbSet<ModelGuildQueueItem> GuildQueueItems { get; set; }
        public DbSet<ModelCachedUser> CachedUsers { get; set; }
        public DbSet<ModelGuildQueuePlaylist> GuildQueuePlaylists { get; set; }

        public DbSet<ModelArchivedTrack> ArchivedTracks { get; set; }
     

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

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            updateChangeTrackerDates();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override int SaveChanges()
        {
            updateChangeTrackerDates();
            return base.SaveChanges();
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
            var entries = ChangeTracker
                .Entries()
                .Where(e => (e.Entity is BaseDbEntity)
                        && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                    ((BaseDbEntity)entry.Entity).CreatedAt = DateTime.Now;

                else if (entry.State == EntityState.Modified)
                    ((BaseDbEntity)entry.Entity).UpdateAt = DateTime.Now;
            }
        }
        #region Entity Framework: Model Configuration and Events
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ModelTempRoom>()
                  .HasKey(e => e.userId);
            /* Cached user */
            {
                modelBuilder.Entity<ModelCachedUser>()
                    .Property(t => t.Id)
                    .ValueGeneratedOnAdd();
                modelBuilder.Entity<ModelCachedUser>()
                    .ToTable("CachedUser")
                    .HasKey(t => t.Id);
            }

            modelBuilder.Entity<ModelGuild>()
                .ToTable("Guild")
                .HasKey(t => t.Id);

            modelBuilder.Entity<ModelGuildQueueItem>()
                .ToTable("GuildQueuePlaylists")
                .HasKey(t => t.Id);

            /* Archived Tracks */
            {
                modelBuilder.Entity<ModelArchivedTrack>()
                    .Property(t => t.Id)
                    .ValueGeneratedOnAdd();

                modelBuilder.Entity<ModelArchivedTrack>()
                    .ToTable("ArchivedTracks")
                    .HasKey(t => t.Id);
            }

            /* Guild Queue Item */
            {
                modelBuilder.Entity<ModelGuildQueueItem>()
                    .Property(t => t.Id)
                    .ValueGeneratedOnAdd();

                modelBuilder.Entity<ModelGuildQueueItem>()
                    .HasOne(qi => qi.RequestedBy)
                    .WithMany(usr => usr.RequestedSongs)
                    .HasForeignKey(qi => qi.RequestedById)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull);

                modelBuilder.Entity<ModelGuildQueueItem>()
                    .HasOne(qi => qi.DeleteBy)
                    .WithMany(usr => usr.DeletedSong)
                    .HasForeignKey(qi => qi.DeletedById)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull);

                modelBuilder.Entity<ModelGuildQueueItem>()
                    .HasOne(qi => qi.Playlist)
                    .WithMany(pl => pl.Songs)
                    .HasForeignKey(qi => qi.PlaylistId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull);

                modelBuilder.Entity<ModelGuildQueueItem>()
                    .HasOne(qi => qi.Guild)
                    .WithMany(guild => guild.Tracks)
                    .HasForeignKey(qi => qi.GuildId)
                    .IsRequired(true)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<ModelGuildQueueItem>()
                    .ToTable("GuildQueueItems")
                    .HasKey(t => t.Id);
            }
/*
            OnModelCreatingPartial(modelBuilder);*/

        }

/*        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);*/
        
        #endregion
    }
}
