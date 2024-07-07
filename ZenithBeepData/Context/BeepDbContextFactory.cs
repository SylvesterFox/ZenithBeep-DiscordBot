using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace ZenithBeepData.Context
{
    public class BeepDbContextFactory : IDesignTimeDbContextFactory<BeepDbContext>
    {
      
        public BeepDbContext CreateDbContext(string[] args)
        {
            

            string pathDb = DbSettings.LocalPathDB();

            var optionsBuilder = new DbContextOptionsBuilder()
                .UseSqlite($"Data Source={pathDb}");

            return new BeepDbContext(optionsBuilder.Options);
        }
    }
}
