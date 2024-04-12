
using System.ComponentModel.DataAnnotations;

namespace ZenithBeepData.Models
{
    public class ModelRooms : BaseDbEntity
    {
        [Key]
        public ulong OwnerChannelId { get; set; }
        public string Name { get; set; }
        public int limit { get; set; }
    }
}
