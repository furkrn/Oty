namespace Oty.Bot;

[PublicAPI]
public interface IClientCollection : IReadOnlyCollection<IClient>
{
    [PublicAPI]
    IClient? this[int shardId] { get; } 

    [PublicAPI]
    Task<IClient> PushAndConnectAsync();

    [PublicAPI]
    Task<bool> DisconnectAndPopAsync();

    [PublicAPI]
    bool TryPeek([MaybeNullWhen(false)] out IClient? client);
}