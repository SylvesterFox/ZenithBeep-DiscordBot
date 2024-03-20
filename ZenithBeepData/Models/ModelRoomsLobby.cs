using System.ComponentModel.DataAnnotations;

namespace ZenithBeepData.Models
{
    public class ModelRoomsLobby : BaseDbEntity
    {
        [Key]
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ModelGuild Guild { get; set; }
        public ulong lobby_id { get; set; }

      
    }
}
