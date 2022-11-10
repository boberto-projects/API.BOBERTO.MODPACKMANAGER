using MinecraftServer.Api.MongoEntities;
using System.Text.Json.Serialization;

namespace MinecraftServer.Api.RequestModels
{
    public class LauncherVersionRequest
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("system")]
        public string System { get; set; }

        [JsonPropertyName("arch")]
        public string Arch { get; set; }
        public LauncherVersionModel ToMap()
        {
            return new LauncherVersionModel()
            {
                Name = this.Version,
                Arch = this.Arch,
                System = this.System,
                Version = this.Version
            };
        }
        

    }
}
