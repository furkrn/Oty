using Oty.Things.AsyncMonitor.AsyncEvents;

namespace Oty.Bot.Core;

[PublicAPI]
public sealed class OtyBotClient : IClient
{
    private readonly DiscordClient _discordClient;

    private readonly IOtyCommandsExtension _commands;

    private readonly IAsyncOptionsMonitor<BotConfiguration> _options;

    private readonly IAddonService _addonService;

    private readonly ICommandsRegisterer _globals;

    private readonly List<IDisposable> _disposables = new();

    private readonly ConcurrentQueue<DiscordStatus> _statusCache = new();

    public OtyBotClient(IServiceProvider services, int shardId)
    {
        this._options = services.GetRequiredService<IAsyncOptionsMonitor<BotConfiguration>>();
        this._discordClient = new DiscordClient(new()
        {
            Token = this.Configuration.SecretToken,
            ShardCount = this.Configuration.ShardCount,
            ShardId = shardId,
            Intents = DiscordIntents.All,
            LoggerFactory = services.GetRequiredService<ILoggerFactory>(),
        });

        if (shardId is 0)
        {
            this._disposables.Add(this._options.OnChange(this.UpdatedConfiguration));
        }

        this._discordClient.AddInteractivity();

        this._commands = this._discordClient.AddCommandsExtension(new()
        {
            RegisteredServices = services,
            Executor = new LimitedExecutor(),
        });

        this._globals = services.GetRequiredService<ICommandsRegisterer>();

        var eventRegisterer = services.GetRequiredService<IAsyncEventRegisterer>();

        this._disposables.AddRange(new[]
        {
            eventRegisterer.RegisterEvents(this._discordClient),
            eventRegisterer.RegisterEvents(this._commands),
            this._commands.InitializeInteractionCommands(new()
            {
                PublishPublishables = false,
                PublishWhenClientReady = false,
            }),
        });

        this._addonService = services.GetRequiredService<IAddonServiceFactory>()
            .Create(this._commands);
    }

    private BotConfiguration Configuration => this._options.CurrentValue;

    public int ShardId => this._discordClient.ShardId;

    public async Task ConnectAsync()
    {
        await this._globals.RegisterCommandsAsync(this._commands);

        var status = new DiscordStatus(this.Configuration.Activity, this.Configuration.ActivityStatus, this.Configuration.UserStatus);
        this._statusCache.Enqueue(status);

        await this._discordClient.ConnectAsync(status);
    }

    public Task DisconnectAsync()
    {
        return this._discordClient.DisconnectAsync();
    }

    public void Dispose()
    {
        foreach (var disposable in this._disposables)
        {
            disposable.Dispose();
        }

        this._addonService.Dispose();
        this._commands.Dispose();
        this._discordClient.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var disposable in this._disposables)
        {
            disposable.Dispose();
        }
        
        await this._addonService.DisposeAsync();
        this._commands.Dispose();
        this._discordClient.Dispose();
    }

    private async Task UpdatedConfiguration(IAsyncOptionsMonitor<BotConfiguration> sender, AsyncEventBox<BotConfiguration> e)
    {
        BotConfiguration configuration = e;

        if (!this._statusCache.TryPeek(out var oldStatus))
        {
            return;
        }

        var newStatus = new DiscordStatus(configuration.Activity, configuration.ActivityStatus, configuration.UserStatus);

        if (oldStatus == newStatus)
        {
            return;
        }

        await this._discordClient.UpdateStatusAsync(newStatus);
        this._statusCache.TryDequeue(out _);
        this._statusCache.Enqueue(newStatus);
    }
}