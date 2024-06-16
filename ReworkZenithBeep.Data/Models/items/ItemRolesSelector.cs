

using System.ComponentModel.DataAnnotations;

namespace ReworkZenithBeep.Data.Models.items
{
    public class ItemRolesSelector
    {
        [Key]
        public int keyId { get; set; }
        public ulong Id { get; set; }
        public ItemGuild Guild { get; set; }
        public ulong messageId { get; set; }
        public ulong roleId { get; set; }
        public ulong channelId { get; set; }
        public string emojiButton { get; set; }
    }
}
