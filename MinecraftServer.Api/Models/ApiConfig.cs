namespace MinecraftServer.Api.Models
{
    public class ApiConfig
    {
        public string Hostname { get; set; }
        public bool Swagger { get; set; }
        public string ModPackDir { get; set; }
        public string ModPackUrl { get; set; }
        public string LauncherVersionDir { get; set; }
        public string LauncherVersionUrl { get; set; }
        public Authorization Authorization { get; set; }
    }

    public class Authorization
    {
        public bool Activate { get; set; }
        public string ApiHeader { get; set; }
        public string ApiKey { get; set; }
    }
}
