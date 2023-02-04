namespace Oty.Bot;

[Serializable]
public sealed class SetupException : Exception
{
    public SetupException(ulong guildId)
    {
        this.GuildId = guildId;
    }

    public SetupException(string? message, ulong guildId) : base(message)
    {
        this.GuildId = guildId;
    }

    public SetupException(string? message, ulong guildId, Exception innerException) : base(message, innerException)
    {
        this.GuildId = guildId;
    }

    private SetupException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ulong GuildId { get; }
}