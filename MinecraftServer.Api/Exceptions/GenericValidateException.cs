namespace MinecraftServer.Api.Exceptions
{
    [Serializable]
    public class CasimiroException : Exception
    {       
            public ExceptionType Type { get; set; }
            public CasimiroException() : base() { }
            public CasimiroException(ExceptionType type, string message) : base(message)
             {
                 this.Type = type;
             }
            public CasimiroException(ExceptionType type, string message, Exception inner) : base(message, inner) { }
    }

    public enum ExceptionType
    {
        Validacao = 400,
        Negocio = 400,
        Generico = 500,
    }



}
