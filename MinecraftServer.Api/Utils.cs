using MinecraftServer.Api.MongoEntities;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MinecraftServer.Api
{
    public static class Utils
    {
        
        public static List<ModPackFileInfo> ListarArquivosRecursivos(ModPackModel modpack)
        {
            var caminho = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Config.CaminhoModPacks, modpack.Directory);

            if (!Directory.Exists(caminho))
            {
                Directory.CreateDirectory(caminho);
            }

            var listaArquivos = new List<ModPackFileInfo>();
           
            var arquivos = Directory.EnumerateFiles(caminho, "*", SearchOption.AllDirectories);

            foreach(var item in arquivos)
            {
                var modpackInfo = new ModPackFileInfo(item);
                modpackInfo.CorrigirCaminho(modpack.Directory, item.Replace(caminho + @"/", ""));
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
        VERIONSCUSTOM,

        [EnumMember(Value = "FILE")]
        FILE,

        [EnumMember(Value = "LIBRARY")]
        LIBRARY
    }


    public string Path { get; set; }
    public decimal Size { get; set; }
    public string Sha1 { get; set; }
    public string Url { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TypeEnum Type { get; set; }

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
            return TypeEnum.VERIONSCUSTOM;
        }
        else
        {
            return TypeEnum.FILE;
        }
    }

    public void CorrigirCaminho(string modpack_dir, string path)
    {
        Path = path;
        Url = $"http://localhost:5000/files/{modpack_dir}/{path}";
    }
    public ModPackFileInfo()
    {

    }
    public ModPackFileInfo(string path)
    {
        FileInfo fsi = new FileInfo(path);
        Type = ObterTipo(path);
        Size = fsi.Length;
        Sha1 = ChecksumUtil.GetChecksum(HashingAlgoTypes.SHA1, path).ToLower();
    }
}


public static class ChecksumUtil
{
    public static string GetChecksum(HashingAlgoTypes hashingAlgoType, string filename)
    {
        using (var hasher = System.Security.Cryptography.HashAlgorithm.Create(hashingAlgoType.ToString()))
        {
            using (var stream = System.IO.File.OpenRead(filename))
            {
                var hash = hasher.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }
    }

}
public enum HashingAlgoTypes
{
    MD5,
    SHA1,
    SHA256,
    SHA384,
    SHA512
}