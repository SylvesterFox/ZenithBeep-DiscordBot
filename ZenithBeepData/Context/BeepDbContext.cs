
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
    }
}
