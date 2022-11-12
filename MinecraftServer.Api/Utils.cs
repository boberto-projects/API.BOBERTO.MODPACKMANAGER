using MinecraftServer.Api.Models;
using MinecraftServer.Api.MongoEntities;

namespace MinecraftServer.Api
{
    public static class Utils
    {

        public static List<ModPackFileInfo> ListarArquivosRecursivos(ApiConfig apiConfig, ModPackModel modpack)
        {
            var caminho = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, apiConfig.ModPackDir, modpack.Directory);

            if (!Directory.Exists(caminho))
            {
                Directory.CreateDirectory(caminho);
            }

            var listaArquivos = new List<ModPackFileInfo>();

            var arquivos = Directory.EnumerateFiles(caminho, "*", SearchOption.AllDirectories);
            var modpacksUrl = new UriBuilder(apiConfig.Hostname);
            modpacksUrl.Path = apiConfig.ModPackUrl;

            foreach (var item in arquivos)
            {
                var modpackInfo = new ModPackFileInfo(item);
                modpackInfo.CorrigirCaminho(modpack.Directory, item.Replace(caminho + @"/", ""), modpacksUrl.ToString());
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
        /// <summary>
        /// https://learn.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="destinationDir"></param>
        /// <param name="recursive"></param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
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