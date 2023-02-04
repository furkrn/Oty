namespace Oty.Bot.Core;

[PublicAPI]
public class OtyHostedService : IHostedService
{
    private readonly BotConfiguration _configuration;

    private readonly IClientCollection _clientCollection;

    private readonly IHostApplicationLifetime _lifeTime;

    private readonly IAsyncMonitorStarter _starter;

    public OtyHostedService(IOptions<BotConfiguration> configuration, IClientCollection clientCollection, IHostApplicationLifetime lifeTime, IAsyncMonitorStarter monitor)
    {
        this._configuration = configuration.Value;
        this._clientCollection = clientCollection;
        this._lifeTime = lifeTime;
        this._starter = monitor;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var taskList = new List<Task>();

        for (int i = 0; i < this._configuration.ShardCount; i++)
        {
            taskList.Add(this._clientCollection.PushAndConnectAsync());
        }

        await (await Task.WhenAny(taskList));

        await this._starter.StartAllAsync(this._lifeTime.ApplicationStopping);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        while (this._clientCollection.Count is not 0)
        {
            await this._clientCollection.DisconnectAndPopAsync();
        }
    }
}
