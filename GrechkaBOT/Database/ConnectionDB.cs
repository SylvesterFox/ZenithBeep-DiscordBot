using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrechkaBOT.Database
{
    public class ConnectionDB
    {
        private readonly IConfigurationRoot _config;
        private static string _con = "User ID=postgres;Password=0270;Host=localhost;Port=5432;Database=grechkadb";

        public ConnectionDB(IConfigurationRoot config)
        {
            _config = config;
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
