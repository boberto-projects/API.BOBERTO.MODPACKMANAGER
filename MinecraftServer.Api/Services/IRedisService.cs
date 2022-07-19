namespace MinecraftServer.Api.Services
{
    public interface IRedisService
    {

        T Get<T>(string chave);
        T Set<T>(string chave, T valor, int expiracao);
        T Set<T>(string chave, T valor);
        bool Apagar(string chave);
        bool Verificar(string chave);

    }
}
