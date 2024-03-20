

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenithBeepData.Models
{
    public class ModelArchivedTrack 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public ulong Id { get; set; }

        public ulong MessageId { get; set; }

        public DateTime DateMessageCreated { get; set; }

        #region ModelGuildQueueItem
        public ulong Position {  get; set; }

        public string Title { get; set; }

        public TimeSpan Length { get; set; }
        public string TrackString { get; set; }

        [ForeignKey("Id")]
        public ulong? RequestedById { get; set; }
        public virtual ModelCachedUser CachedUser { get; set; }

        [ForeignKey("Id")]
        public ulong GuildId { get; set; }
        public virtual ModelGuild Guild { get; set; }

        #endregion
    }
}
