using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ReworkZenithBeep.Data.Models.items;


namespace ReworkZenithBeep.Data
{
    public class BotContext : DbContext
    {
        public BotContext(DbContextOptions<BotContext> options) : base(options) { }


        public DbSet<ItemGuild> Guilds { get; set; }
        public DbSet<ItemRolesSelector> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemGuild>()
                .HasMany(e => e.Roles)
                .WithOne(e => e.Guild)
                .HasForeignKey(e => e.Id)
                .IsRequired();

            modelBuilder.Entity<ItemRolesSelector>()
                .HasOne(e => e.Guild)
                .WithMany(e => e.Roles)
                .HasForeignKey(e => e.Id)
                .IsRequired();
        }
    }

    public class BotContextFactory : IDesignTimeDbContextFactory<BotContext>
    {
        public BotContext CreateDbContext(string[] atgs)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BotContext>();
            string tokendb = "server=localhost;database=Zenith;User=root;Password=8342003";
            optionsBuilder.UseMySql(tokendb, ServerVersion.AutoDetect(tokendb), x => x.MigrationsAssembly("ReworkZenithBeep.Data.Migrations"));
            return new BotContext(optionsBuilder.Options);
        }
    }
}
