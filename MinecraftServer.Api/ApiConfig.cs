namespace MinecraftServer.Api
{   
    public class ApiConfig
    {
        public string CaminhoModPacks { get; set; }
        public string CaminhoLauncherVersion  { get; set; }
        public string ModPackDirUrl { get; set; }
        public string VersionDirUrl { get; set; }
        public Authorization Authorization { get; set; }
    }

    public class Authorization
    {
        public bool Activate { get; set; }
        public string ApiHeader { get; set; }
        public string ApiKey { get; set; }
    }
}
