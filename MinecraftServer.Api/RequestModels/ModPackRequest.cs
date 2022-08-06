
using MinecraftServer.Api.MongoEntities;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace MinecraftServer.Api.RequestModels
{
    public class ModPackRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("datetimeCreatAt")]
        public DateTime DatetimeCreatAt { get; set; }

        [JsonPropertyName("datetimeUpdatAt")]
        public DateTime DatetimeUpdatAt { get; set; }

        [JsonPropertyName("isDefault")]
        public bool IsDefault { get; set; }

        [JsonPropertyName("isVerifyMods")]
        public bool IsVerifyMods { get; set; }

        [JsonPropertyName("isPremium")]
        public bool IsPremium { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("directory")]
        public string Directory { get; set; }

        [JsonPropertyName("forgeVersion")]
        public string ForgeVersion { get; set; }

        [JsonPropertyName("gameVersion")]
        public string GameVersion { get; set; }

        [JsonPropertyName("img")]
        public string Img { get; set; }

        [JsonPropertyName("serverIp")]
        public string ServerIp { get; set; }

        [JsonPropertyName("serverPort")]
        public int ServerPort { get; set; }

        public ModPackModel ToMap()
        {
            return new ModPackModel
            {
                Name = this.Name,
                Author = this.Author,
                DateTimeCreatAt = this.DatetimeCreatAt,
                DateTimeUpdatAt = this.DatetimeUpdatAt,
                IsDefault = this.IsDefault,
                IsVerifyMods = this.IsVerifyMods,
                IsPremium = this.IsPremium,
                Description = this.Description,
                Directory = this.Directory,
                ForgeVersion = this.ForgeVersion,
                GameVersion = this.GameVersion,
                Img = this.Img,
                ServerIp = this.ServerIp,
                ServerPort = this.ServerPort,
            };
        }

    }
}
