using MinecraftServer.Api.MongoEntities;
using System.Text.Json.Serialization;

namespace MinecraftServer.Api.RequestModels
{
    public class LauncherVersionRequest
    {
        public class Linux64Model
        {
            [JsonPropertyName("url")]
            public string Url { get; set; }
        }

        public class Mac64Model
        {
            [JsonPropertyName("url")]
            public string Url { get; set; }
        }

        public class Win64Model
        {
            [JsonPropertyName("url")]
            public string Url { get; set; }
        }

        public class PackagesModel
        {
            [JsonPropertyName("win64")]
            public Win64Model Win64 { get; set; }

            [JsonPropertyName("mac64")]
            public Mac64Model Mac64 { get; set; }

            [JsonPropertyName("linux64")]
            public Linux64Model Linux64 { get; set; }
        }
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("packages")]
        public PackagesModel Packages { get; set; }

        public LauncherVersionModel ToMap()
        {
            return new LauncherVersionModel()
            {
                Packages = new LauncherVersionModel.PackagesEntity()
                {
                    Win64 = new LauncherVersionModel.Win64Entity()
                    {
                        Url = this.Packages.Win64.Url
                    },
                    Linux64 = new LauncherVersionModel.Linux64Entity()
                    {
                        Url = this.Packages.Linux64.Url
                    },
                    Mac64 = new LauncherVersionModel.Mac64Entity()
                    {
                        Url = this.Packages.Mac64.Url
                    }
                },
                Version = this.Version
            };
        }
        

    }
}
