using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace MinecraftServer.Api.Models
{
    public class ModPackModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Author { get; set; } = null!;
        public DateTime DatetimeCreatAt { get; set; }
        public DateTime DatetimeUpdatAt { get; set; }
        public bool VerifyMods { get; set; }
        public bool Default { get; set; }
        public string Description { get; set; } = null!;
        public string Directory { get; set; } = null!;
        public string ForgeVersion { get; set; } = null!;
        public string FabricVersion { get; set; } = null!;
        public string GameVersion { get; set; } = null!;
        public string Img { get; set; } = null!;
        public bool Premium { get; set; }
        public string ServerIp { get; set; } = null!;
        public int ServerPort { get; set; }
    }
}
