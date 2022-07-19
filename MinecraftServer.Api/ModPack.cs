using System.Text.Json.Serialization;

namespace MinecraftServer.Api.Models
{
    public class ModPack
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("datetime_creat_at")]
        public DateTime DatetimeCreatAt { get; set; }

        [JsonPropertyName("datetime_updat_at")]
        public DateTime DatetimeUpdatAt { get; set; }

        [JsonPropertyName("default")]
        public bool Default { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("directory")]
        public string Directory { get; set; }

        [JsonPropertyName("forge_version")]
        public string ForgeVersion { get; set; }

        [JsonPropertyName("game_version")]
        public string GameVersion { get; set; }

        [JsonPropertyName("img")]
        public string Img { get; set; }

        [JsonPropertyName("premium")]
        public bool Premium { get; set; }

        [JsonPropertyName("server_ip")]
        public string ServerIp { get; set; }

        [JsonPropertyName("server_port")]
        public string ServerPort { get; set; }
    }
}
