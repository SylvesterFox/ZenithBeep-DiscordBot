
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
    }
}
