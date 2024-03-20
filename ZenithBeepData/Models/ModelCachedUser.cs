
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ZenithBeepData.Models
{
    public class ModelCachedUser : BaseDbEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }

        [Required]
        public ulong UserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [ForeignKey("Id")]
        public ulong GuildId { get; set; }
        public virtual ModelGuild ModelGuild { get; set; }

        public ICollection<ModelGuildQueueItem> RequestedSongs { get; set; }
        public ICollection<ModelGuildQueueItem> DeletedSong { get; set; }

        public ICollection<ModelArchivedTrack> ArchivedTracks { get; set; }
    }
}
