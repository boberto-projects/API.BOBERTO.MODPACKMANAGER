using MinecraftServer.Api.Models;

namespace MinecraftServer.Api.RequestModels
{
    public class ModPackRequest
    {
        public string Name { get; set; }

        public string Author { get; set; }

        public DateTime DatetimeCreatAt { get; set; }

        public DateTime DatetimeUpdatAt { get; set; }

        public bool IsDefault { get; set; }

        public bool IsVerifyMods { get; set; }

        public bool IsPremium { get; set; }

        public string Description { get; set; }

        public string Directory { get; set; }

        public string ForgeVersion { get; set; }

        public string GameVersion { get; set; }

        public string Img { get; set; }

        public string ServerIp { get; set; }

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
