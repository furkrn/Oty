namespace Oty.Bot;

[PublicAPI]
public interface IClient : IAsyncDisposable, IDisposable
{
    [PublicAPI]
    int ShardId { get; }

    [PublicAPI]
    Task ConnectAsync();

    [PublicAPI]
    Task DisconnectAsync();
}