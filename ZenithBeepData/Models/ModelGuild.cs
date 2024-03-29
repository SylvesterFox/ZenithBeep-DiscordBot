﻿

using System.ComponentModel.DataAnnotations;

namespace ZenithBeepData.Models
{
    public class ModelGuild
    {
        [Key]
        public int Id { get; set; }
        public ulong guildId {  get; set; }
        public string Lang { get; set; } = "EN-us";
        public string Prefix { get; set; } = "!";

        public ICollection<ModelRoles> Roles { get; set; } = new List<ModelRoles>();
        public ICollection<ModelRoomsLobby> Lobbys { get; set; } = new List<ModelRoomsLobby>();

    }
}
