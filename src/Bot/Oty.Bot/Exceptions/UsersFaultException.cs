namespace Oty.Bot;

[Serializable]
public sealed class UsersFaultException : Exception
{
    public UsersFaultException(string? message, Exception innerException) : base(message, innerException)
    {
        ArgumentNullException.ThrowIfNull(innerException, nameof(innerException));
    }

    private UsersFaultException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public static void ThrowFrom(Exception innerException)
    {
        throw new UsersFaultException("An user related exception has thrown", innerException);
    }
}