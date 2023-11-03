using Microsoft.Extensions.Configuration;

namespace GrechkaBOT.Database
{
    public class DatabasePost : ConnectionDB
    {
        private static readonly string _selectQuery = $@"SELECT id_guild as {nameof(ModelGuild.guildId)}, name_guild as {nameof(ModelGuild.Name)}, lang as {nameof(ModelGuild.Leng)}, id as {nameof(ModelGuild.Id)} FROM guilds WHERE id_guild = @{nameof(ModelGuild.guildId)}";
        private static readonly string _insertRole = $@"INSERT INTO roles 
        (
            id_guilds, 
            emoji, 
            id_massage,  
            id_channel, 
            role_name, 
            role_id
        ) values  
        (
            @{nameof(ModelRoles.guilds_id_KEY)},
            @{nameof(ModelRoles.setEmoji)},
            @{nameof(ModelRoles.messageId)},
            @{nameof(ModelRoles.channelId)},
            @{nameof(ModelRoles.roleName)},
            @{nameof(ModelRoles.roleId)}
        )";

        private static readonly string _insertLobby = $@"INSERT INTO roomers_lobbys 
        (
            id_guilds,
            id_lobby
        ) values 
        (
            @{nameof(ModelRoomsLobby.guild_key)},
            @{nameof(ModelRoomsLobby.lobby_id)}
        )";

        private static readonly string _selectRole = $@"SELECT 
        emoji as {nameof(ModelRoles.setEmoji)},  
        id_massage as {nameof(ModelRoles.messageId)},
        id_channel as {nameof(ModelRoles.channelId)},
        role_name as {nameof(ModelRoles.roleName)},
        role_id as {nameof(ModelRoles.roleId)},
        id_guilds as {nameof(ModelRoles.guilds_id_KEY)}
        FROM roles WHERE id_massage = @{nameof(ModelRoles.messageId)} AND emoji = @{nameof(ModelRoles.setEmoji)}";

        private static readonly string _selectQueryLobby = $@"SELECT id_lobby as {nameof(ModelRoomsLobby.lobby_id)} FROM roomers_lobbys WHERE id_guilds = @{nameof(ModelRoomsLobby.guild_key)}";

        public DatabasePost(IConfigurationRoot config) : base(config)
        {
        }

        public static int insertGuild(object guilds)
        {
            string _insertQuery = $@"INSERT INTO guilds (name_guild, id_guild, lang) values (@{nameof(ModelGuild.Name)}, @{nameof(ModelGuild.guildId)}, @{nameof(ModelGuild.Leng)})";
            return Execute(_insertQuery, guilds);
            
        }

        public static int insertRoles(object roles)
        {
            return Execute(_insertRole, roles);
        }

        public static int insertLobby(object lobby) 
        {
            return Execute(_insertLobby, lobby);
        }

        public static int deleteRoles(object roles)
        {
            ModelRoles res = GetRole<ModelRoles>(roles);
            var delete = new ModelRoles
            {
                guilds_id_KEY = res.guilds_id_KEY,
                setEmoji = res.setEmoji,
                messageId = res.messageId
            };
            string _deleteQuery = $@"DELETE FROM roles WHERE emoji = @{nameof(ModelRoles.setEmoji)} AND id_guilds = @{nameof(ModelRoles.guilds_id_KEY)} AND id_massage = @{nameof(ModelRoles.messageId)}";
            
            return Execute(_deleteQuery, delete);
        }

        public static int deleteLobby(long lobbyId) 
        {
            var delete = new ModelRoomsLobby 
            {
                lobby_id = lobbyId
            };

            string _deleteQuery = $@"DELETE FROM roomers_lobbys WHERE id_lobby = @{nameof(ModelRoomsLobby.lobby_id)}";

            return Execute(_deleteQuery, delete);
        }

        public static ModelGuild GetGuild<ModelGuild>(object guild)
        {
            var res = QueryFirstOrDefault<ModelGuild>(_selectQuery, guild);
            return res;
        }

        public static ModelRoomsLobby GetIdChannelLobby<ModelRoomsLobby>(object lobby) 
        {
            var res = QueryFirstOrDefault<ModelRoomsLobby>(_selectQueryLobby, lobby);
            return res;
        }

        public static ModelRoles GetRole<ModelRoles>(object role)
        {
            var res = QueryFirstOrDefault<ModelRoles>(_selectRole, role);
            return res;
        }
    }
}
