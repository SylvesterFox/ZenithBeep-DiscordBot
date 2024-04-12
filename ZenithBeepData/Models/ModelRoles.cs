

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenithBeepData.Models
{
    public class ModelRoles : BaseDbEntity
    {
        [Key]
        public ulong Id { get; set; }

        [ForeignKey("Id")]
        public ulong GuildId { get; set; }
        public ModelGuild Guild { get; set; }


        public ulong messageId { get; set; }
        public ulong roleId { get; set; }
        public ulong channelId { get; set; }
        public string setEmoji {  get; set; }
    }
}
