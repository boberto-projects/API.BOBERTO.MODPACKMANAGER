using MinecraftServer.Api.MongoEntities;
using System.Text.Json.Serialization;

namespace MinecraftServer.Api.RequestModels
{
    public class LauncherVersionRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        //[JsonPropertyName("files")]
        //public List<string> Files { get; set; }

        [JsonPropertyName("system")]
        public string System { get; set; }

        [JsonPropertyName("arch")]
        public string Arch { get; set; }
        public LauncherVersionModel ToMap()
        {
            return new LauncherVersionModel()
            {
                Name = this.Name,
                Arch = this.Arch,
                System = this.System,
                Version = this.Version
            };
        }
    }
}
