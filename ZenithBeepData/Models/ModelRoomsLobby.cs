using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenithBeepData.Models
{
    public class ModelRoomsLobby
    {
        [Key]
        public ulong lobby_id { get; set; }
        public ulong GuildId { get; set; }
        public ModelGuild Guild { get; set; }

      
    }
}
