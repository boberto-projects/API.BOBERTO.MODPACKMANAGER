namespace MinecraftServer.Api.Exceptions
{
    [Serializable]
    public class GenericValidateException : Exception
    {       
            public ExceptionType Type { get; set; }
            public GenericValidateException() : base() { }
            public GenericValidateException(ExceptionType type, string message) : base(message)
             {
                 this.Type = type;
             }
            public GenericValidateException(ExceptionType type, string message, Exception inner) : base(message, inner) { }
    }

    public enum ExceptionType
    {
        Validacao = 400,
        Generico = 500,
    }



}
