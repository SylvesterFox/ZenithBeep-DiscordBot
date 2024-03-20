
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenithBeepData.Models
{
    public class ModelGuild : BaseDbEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong Id { get; set; }
        public ulong guildId {  get; set; }
        public string guildName { get; set; }
        public string Lang { get; set; } = "EN-us";
        public string Prefix { get; set; } = "!";

        public ulong? MusicChannelId { get; set; }
        public string? MusicChannelName { get; set; }

        public ulong? LastMessageStatusId { get; set; }

        public ulong NextTrack { get; set; } = 1;

        public ulong TrackCount { get; set; } = 0;

        public ulong CurrentTrack { get; set; }

        public bool IsPlaying { get; set; }


        public bool LeaveAfterQueue { get; set; }

        public ICollection<ModelRoles> Roles { get; set; } = new List<ModelRoles>();
        public ICollection<ModelRoomsLobby> Lobbys { get; set; } = new List<ModelRoomsLobby>();

        public ICollection<ModelGuildQueueItem> Tracks { get; set; }

    }
}
