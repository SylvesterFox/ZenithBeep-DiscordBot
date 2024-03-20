
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenithBeepData.Models
{
    public class ModelGuildQueuePlaylist : BaseDbEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }
        public string Title { get; set; }
        public int PlaylistSongConut { get; set; }

        [ForeignKey("Id")]
        public ulong? CreatedById { get; set; }
        public virtual ModelCachedUser CreatedBy { get; set; }

        public ICollection<ModelGuildQueueItem> Songs { get; set; }
    }
}
