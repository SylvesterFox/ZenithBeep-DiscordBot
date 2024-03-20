

using Microsoft.EntityFrameworkCore;

namespace ZenithBeepData.Models
{
    public class ModelTempRoom : BaseDbEntity
    {
    
        public ulong userId { get; set; }   
        public ulong channelRoomId { get; set; }

    }
}
