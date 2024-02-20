using DotNetEnv.Configuration;
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
               .AddDotNetEnv("../.env");

            var _config = config.Build();

            var optionsBuilder = new DbContextOptionsBuilder()
                .UseNpgsql($"Host={_config["POSTGRES_HOST"]};Database={_config["POSTGRES_DB"]};Username={_config["POSTGRES_USER"]};Password={_config["POSTGRES_PASSWORD"]};");

            return new BeepDbContext(optionsBuilder.Options);
        }

       
    }
}
