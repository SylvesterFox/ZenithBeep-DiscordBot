

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ZenithBeepData.Models
{
    public class ModelTempRoom
    {
        [Key]
        public ulong userId { get; set; }   
        public ulong channelRoomId { get; set; }

    }
}
