namespace Oty.Bot;

[Serializable]
public sealed class LimitedException : Exception
{
    public LimitedException()
    {
    }

    public LimitedException(string? message) : base(message)
    {
    }

    public LimitedException(string? message, Exception innerException) : base(message, innerException)
    {
    }

    private LimitedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}