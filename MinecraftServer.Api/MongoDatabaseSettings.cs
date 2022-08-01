namespace MinecraftServer.Api
{
    public class MongoDatabaseSettings
    {
        public IEnumerable<CollectionSettings> CollectionSettings { get; set; }

        public bool ColecacaoExiste(string valor) => CollectionSettings.Any(e => e.CollectionName.Equals(valor));

        public CollectionSettings ObterPorColecao(string valor) => CollectionSettings.First(e => e.CollectionName.Equals(valor));
    }

    public class CollectionSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; } 
        public string CollectionName { get; set; }
    }

}
