using System.Text.Json.Serialization;

namespace MinecraftServer.Api.Models
{
    //gambiarra por agora. O boberto foi estruturado as pressas e certas variáveis não deveriam nem existir
    //como o objetivo é migrar do PHP pra cá, a comunicação dos serviços do Boberto também serão alterados.
    public class ModPackFileInfo
    {
        public string Path { get; set; }
        public decimal Size { get; set; }
        public string Sha1 { get; set; }
        public string Url { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FolderType Type { get; set; }
        private FolderType ObterTipo(string path)
        {
            if (path.Contains("libraries"))
            {
                return FolderType.LIBRARY;
            }
            else if (path.Contains("mods"))
            {
                return FolderType.MOD;
            }
            else if (path.Contains("versions"))
            {
                return FolderType.VERSIONCUSTOM;
            }
            return FolderType.FILE;
        }
        public ModPackFileInfo()
        {

        }
        public ModPackFileInfo(string path)
        {
            FileInfo fsi = new FileInfo(path);
            Type = ObterTipo(path);
            Size = fsi.Length;
            Sha1 = Utils.GetChecksum(HashingAlgoTypes.SHA1, path);
        }
        public void CorrigirCaminho(string modpackDir, string filePath, string url)
        {
            Path = filePath;
            Url = $"{url}/{modpackDir}/{filePath}";
        }
    }
    
}
