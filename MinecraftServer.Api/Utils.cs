using Microsoft.Extensions.Options;
using MinecraftServer.Api.Models;
using MinecraftServer.Api.MongoEntities;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MinecraftServer.Api
{
    public static class Utils
    {
        
        public static List<ModPackFileInfo> ListarArquivosRecursivos(IOptions<ApiConfig> apiConfig, ModPackModel modpack)
        {
            var caminho = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, apiConfig.Value.CaminhoModPacks, modpack.Directory);

            if (!Directory.Exists(caminho))
            {
                Directory.CreateDirectory(caminho);
            }

            var listaArquivos = new List<ModPackFileInfo>();
           
            var arquivos = Directory.EnumerateFiles(caminho, "*", SearchOption.AllDirectories);

            foreach(var item in arquivos)
            {
                var modpackInfo = new ModPackFileInfo(item);
                modpackInfo.CorrigirCaminho(modpack.Directory, item.Replace(caminho + @"/", ""), apiConfig.Value.ModPackDirUrl);
                listaArquivos.Add(modpackInfo);
            }
      
            return listaArquivos;
        }

        public static string GetChecksum(HashingAlgoTypes hashingAlgoType, string filename)
        {
            using (var hasher = System.Security.Cryptography.HashAlgorithm.Create(hashingAlgoType.ToString()))
            {
                using (var stream = System.IO.File.OpenRead(filename))
                {
                    var hash = hasher.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLower();
                }
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