using MinecraftServer.Api.Models;
using NETCore.Encrypt;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MinecraftServer.Api.Utils
{
    public static class Utils
    {
        public static IEnumerable<ModPackModel> ObterModPacks()
        {
            var modpacks = File.ReadAllText(Config.Config.CaminhoListaModPacks, Encoding.UTF8);
            var modpack_json = JsonSerializer.Deserialize<IEnumerable<ModPackModel>>(modpacks);
            return modpack_json;
        }
        
        public static List<ModPackFileInfo> ListarArquivosRecursivos(ModPackModel modpack)
        {
            var caminho = Path.Combine(Config.Config.CaminhoModPacks, modpack.Directory);

            var listaArquivos = new List<ModPackFileInfo>();
           
            var arquivos = Directory.EnumerateFiles(caminho, "*", SearchOption.AllDirectories);

            foreach(var item in arquivos)
            {
                var modpackInfo = new ModPackFileInfo(item);
                modpackInfo.CorrigirCaminho(item.Replace(caminho + @"/", ""));
                listaArquivos.Add(modpackInfo);
            }
           
            return listaArquivos;
        }

    }
}


//gambiarra por agora. O boberto foi estruturado as pressas e certas variáveis não deveriam nem existir
//como o objetivo é migrar do PHP pra cá, a comunicação dos serviços do Boberto também serão alterados.

public class ModPackFileInfo
{
    public enum TypeEnum
    {
        [EnumMember(Value = "MOD")]
        MOD,

        [EnumMember(Value = "VERIONSCUSTOM")]
        VERSIONCUSTOM,

        [EnumMember(Value = "FILE")]
        FILE,

        [EnumMember(Value = "LIBRARY")]
        LIBRARY
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public TypeEnum Type { get; set; }
    public decimal Size { get; set; }
    public string Sha1 { get; set; }
    public string Path { get; set; }
    public string Url { get; set; }

    private TypeEnum ObterTipo(string path)
    {
        if (path.Contains("libraries"))
        {
            return TypeEnum.LIBRARY;
        } 
        else if (path.Contains("mods"))
        {
            return TypeEnum.MOD;
        }
        else if (path.Contains("versions"))
        {
            return TypeEnum.VERSIONCUSTOM;
        }
        else
        {
            return TypeEnum.FILE;
        }
    }

    public void CorrigirCaminho(string path)
    {
        Url = $"http://localhost:5000/{path}";
        Path = path;
    }

    public ModPackFileInfo(string path)
    {
        FileInfo fsi = new FileInfo(path);
        Type = ObterTipo(path);
        Size = fsi.Length;
        Sha1 = EncryptProvider.Sha1(path);
    }
}