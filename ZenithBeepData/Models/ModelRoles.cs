

using System.ComponentModel.DataAnnotations;

namespace ZenithBeepData.Models
{
    public class ModelRoles
    {
        [Key]
        public int Id { get; set; }
        public int GuildId { get; set; }
        public ModelGuild Guild { get; set; }
        public ulong messageId { get; set; }
        public ulong roleId { get; set; }
        public ulong channelId { get; set; }
        public string setEmoji {  get; set; }
    }
}
