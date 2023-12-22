using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Serilog;

namespace GrechkaBOT.Database
{
    public class ConnectionDB
    {
        private readonly IServiceProvider _service;
        private static string? _con = null;
        private readonly IConfiguration _config;

        public ConnectionDB(IServiceProvider service)
        {
            _config = service.GetRequiredService<IConfiguration>();
            if (_config["pg_user"].ToString() != null) {
                _con = $"User ID={_config["pg_user"]};Password={_config["password"]};Host=localhost;Port=5432;Database={_config["db"]}";
            } else {
                Log.Error("Not found settings db config --> appsettings.yml");
            }
        }

        public static T QueryFirstOrDefault<T>(string sql, object? perameters = null)
        {
            using (var connection = new NpgsqlConnection(_con))
            {
                return connection.QueryFirstOrDefault<T>(sql, perameters);
            }
        }

        public List<T> Query<T>(string sql, object? parameters = null)
        {
            using (var connection = new NpgsqlConnection(_con))
            {
                return connection.Query<T>(sql, parameters).ToList();
            }
        }

        public static int Execute(string sql, object? parameters = null)
        {
            using (var connection = new NpgsqlConnection(_con))
            {
                return connection.Execute(sql, parameters);
            }
        }
    }
}
