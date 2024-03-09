
using Microsoft.Extensions.CommandLineUtils;
using ZenithBeepData.Context;


namespace ZenithBeepData
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            var migareteOption = app.Option("--applymigrations", "Apply migration", CommandOptionType.NoValue);
            var dbContextFactory = new BeepDbContextFactory();
            var dbContext = dbContextFactory.CreateDbContext(args);
            

            app.OnExecute(() =>
            {
                       
                if (migareteOption.HasValue())
                {
                    Console.WriteLine("Starating apply migrations");
                    dbContext.ApplyMigrations();
                } else
                {
                    Console.WriteLine("Not found args");
                }

                return 0;
            });

            app.Execute(args);

        }
    }
}
