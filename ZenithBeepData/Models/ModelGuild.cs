

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenithBeepData.Models
{
    public class ModelGuild : BaseDbEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string Lang { get; set; } = "EN-us";
        public string Prefix { get; set; } = "!";

        public ICollection<ModelRoles> Roles { get; set; } = new List<ModelRoles>();
        public ICollection<ModelRoomsLobby> Lobbys { get; set; } = new List<ModelRoomsLobby>();

    }
}
