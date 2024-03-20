using DotNetEnv;
using DotNetEnv.Configuration;
using Microsoft.Extensions.Configuration;

namespace ZenithBeepData.Context
{
    public class ConfigDb
    {
        public static IConfiguration buildConfig(string path )
        {
            var config = new ConfigurationBuilder()
             .SetBasePath(path)
             .AddDotNetEnv(".env");

            return config.Build();
        }
    }
}
