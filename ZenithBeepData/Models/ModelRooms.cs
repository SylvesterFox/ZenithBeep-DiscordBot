
using System.ComponentModel.DataAnnotations;

namespace ZenithBeepData.Models
{
    public class ModelRooms : BaseDbEntity
    {
        [Key]
        public int Id { get; set; }
        public ulong owner_channel { get; set; }
        public string channel_name { get; set; }
        public int limit { get; set; }
    }
}
