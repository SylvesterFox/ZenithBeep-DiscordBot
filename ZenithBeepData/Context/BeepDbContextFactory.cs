using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


namespace ZenithBeepData.Context
{
    public class BeepDbContextFactory : IDesignTimeDbContextFactory<BeepDbContext>
    {
      
        public BeepDbContext CreateDbContext(string[] args)
        {
            var config = ConfigDb.buildConfig(Directory.GetCurrentDirectory());

            var optionsBuilder = new DbContextOptionsBuilder()
                .UseNpgsql(config.GetConnectionString("db"));

            return new BeepDbContext(optionsBuilder.Options);
        }

        
    }
}
