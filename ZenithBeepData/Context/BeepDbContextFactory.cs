using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


namespace ZenithBeepData.Context
{
    public class BeepDbContextFactory : IDesignTimeDbContextFactory<BeepDbContext>
    {
        public BeepDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddYamlFile("appsettings.yml");

            var _config = config.Build();

            var optionsBuilder = new DbContextOptionsBuilder()
                .UseNpgsql(_config.GetConnectionString("Default"));

            return new BeepDbContext(optionsBuilder.Options);
        }
    }
}
