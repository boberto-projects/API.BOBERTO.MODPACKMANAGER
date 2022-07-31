namespace MinecraftServer.Api
{
    public class MongoDatabaseSettings
    {
        public IEnumerable<CollectionSettings> CollectionSettings { get; set; }

        public bool ColecacaoExiste()
        {
            return CollectionSettings.Any(e => e.CollectionName.Equals("dev"));
        }

        public CollectionSettings? ObterPorColecao()
        {
            return CollectionSettings.FirstOrDefault(e => e.CollectionName.Equals("dev"));
        }
    }

    public class CollectionSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string CollectionName { get; set; } = string.Empty;
    }

}
