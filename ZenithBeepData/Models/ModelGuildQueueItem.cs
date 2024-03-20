

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZenithBeepData.Context;

namespace ZenithBeepData.Models
{
    public class ModelGuildQueueItem : BaseDbEntity
    {
        [Key]
        public ulong Id { get; set; }

        [ForeignKey("Id")]
        public ulong GuildId { get; set; }
        public virtual ModelGuild Guild { get; set; }

        public ulong Position { get; set; }
        public string Title { get; set; }
        public TimeSpan Length { get; set; }
        public string TrackString { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DateDeleted { get; set; }

        [ForeignKey("Id")]
        public ulong? DeletedById { get; set; }
        public virtual ModelCachedUser DeleteBy { get; set; }
        public string? DeletedReason { get; set; }

        [ForeignKey("Id")]
        public ulong? RequestedById { get; set; }
        public virtual ModelCachedUser RequestedBy { get; set; }

        [ForeignKey("Id")]
        public ulong? PlaylistId { get; set; }
        public virtual ModelGuildQueuePlaylist Playlist { get; set; }

        public async Task<string> GetTagline(BeepDbContext? context = null, bool fetchUserIfNotPresent = false)
        {
            if (context == null && RequestedBy == null)
            {
                
                return ($"`{Title}` at position `{Position}`, requested at "
                    + $"`{CreatedAt:dd/MM/yyyy HH:mm:ss}`");
            }

            if (fetchUserIfNotPresent && RequestedBy == null)
            {
                var userQuery = context?.CachedUsers.Where(x => x.UserId == RequestedById);
                if (userQuery != null && await userQuery.AnyAsync())
                    RequestedBy = await userQuery.FirstAsync();
            }

            if (RequestedBy == null)
            {
                return ($"`{Title}` at position `{Position}`, requested at "
                    + $"`{CreatedAt:dd/MM/yyyy HH:mm:ss}`");
            }

            return $"`{Title}` at position `{Position}`, requested by `{RequestedBy.Username}` at "
                + $"`{CreatedAt:dd/MM/yyyy HH:mm:ss}`";
        }
    }
}
