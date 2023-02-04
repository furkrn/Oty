namespace Oty.Bot;

[PublicAPI]
public class OtyBotClientStack : IClientCollection
{
    private readonly ConcurrentStack<IClient> _stack = new();

    private readonly IServiceProvider _serviceProvider;

    private readonly ILogger<OtyBotClientStack> _logger;

    private readonly IHostApplicationLifetime _lifeTime;

    public OtyBotClientStack(IServiceProvider serviceProvider, ILogger<OtyBotClientStack> logger, IHostApplicationLifetime lifetime)
    {
        this._serviceProvider = serviceProvider;
        this._logger = logger;
        this._lifeTime = lifetime;
    }

    public int Count => this._stack.Count;

    public IClient? this[int index]
    {
        get => this._stack.FirstOrDefault(c => c.ShardId == index);
    }

    public IEnumerator<IClient> GetEnumerator()
    {
        return this._stack.GetEnumerator();
    }

    public async Task<IClient> PushAndConnectAsync()
    {
        int shardId = this.Count;

        var client = new OtyBotClient(this._serviceProvider, shardId);

        this._stack.Push(client);

        await client.ConnectAsync();

        this._logger.LogInformation("A new shinny, glamarious Shard:{0} is now avaliable!", shardId);

        return client;
    }

    public async Task<bool> DisconnectAndPopAsync()
    {
        if (!this._stack.TryPop(out var client))
        {
            return false;
        }

        if (client.ShardId is 0 && !this._lifeTime.ApplicationStopping.IsCancellationRequested)
        {
            this._logger.LogWarning("Cannot remove shard 0 while bot is still running.");

            return false;
        }

        int shardId = client.ShardId;

        try
        {
            this._logger.LogInformation("Disconnecting Shard:{0}", shardId);

            await client.DisconnectAsync();

            this._logger.LogInformation("Disconnected Shard:{0}", shardId);
        }
        finally
        {
            await client.DisposeAsync();

            this._logger.LogInformation("Disposed Shard:{0}", shardId);
        }

        return true;
    }

    public bool TryPeek([MaybeNullWhen(false)] out IClient? client)
    {
        return this._stack.TryPeek(out client);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this._stack.GetEnumerator();
    }
}