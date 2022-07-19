using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace MinecraftServer.Api.Models
{
    public class ModPackModel
    {
        //[JsonPropertyName("id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

       // [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        //  [JsonPropertyName("author")]
        public string Author { get; set; } = null!;

        //   [JsonPropertyName("datetime_creat_at")]
        public DateTime DatetimeCreatAt { get; set; }

        // [JsonPropertyName("datetime_updat_at")]
        public DateTime DatetimeUpdatAt { get; set; } 

        //  [JsonPropertyName("default")]
        public bool Default { get; set; }

        //  [JsonPropertyName("description")]
        public string Description { get; set; } = null!;

        //   [JsonPropertyName("directory")]
        public string Directory { get; set; } = null!;

        //  [JsonPropertyName("forge_version")]
        public string ForgeVersion { get; set; } = null!;

        //  [JsonPropertyName("game_version")]
        public string GameVersion { get; set; } = null!;

        //   [JsonPropertyName("img")]
        public string Img { get; set; } = null!;

        //  [JsonPropertyName("premium")]
        public bool Premium { get; set; }

        //   [JsonPropertyName("server_ip")]
        public string ServerIp { get; set; } = null!;

        //  [JsonPropertyName("server_port")]
        public string ServerPort { get; set; } = null!;
    }
}
