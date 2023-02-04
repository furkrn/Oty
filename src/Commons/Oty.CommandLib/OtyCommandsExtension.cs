namespace Oty.CommandLib;

public sealed class OtyCommandsExtension : BaseExtension, IOtyCommandsExtension
{
    private readonly ConcurrentHashSet<BaseCommandMetadata> _registeredCommands = new();

    private readonly ConcurrentDictionary<HandlerKey, ICommandMetadataReceiver> _receivers = new();

    private readonly OtyCommandsConfiguration _configuration;

    private readonly ICommandExecutor _executor;

    private DiscordClientEventRegisterer? _eventRegisterer;

    private AsyncEvent<IOtyCommandsExtension, CommandHandledEventArgs>? _commandHandledEvent;

    private AsyncEvent<IOtyCommandsExtension, AddedCommandEventArgs>? _addedCommandEvent;

    private AsyncEvent<IOtyCommandsExtension, UpdatedCommandEventArgs>? _updatedCommandEvent;

    private AsyncEvent<IOtyCommandsExtension, RemovedCommandEventArgs>? _removedCommandEvent;

    private bool _isClientReady;

    private bool _disposed;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public OtyCommandsExtension(OtyCommandsConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        this._configuration = configuration;
        this._executor = configuration.Executor ?? new DefaultCommandExecutor();
        this.CommandParser = configuration.CommandParser ?? new DefaultCommandParser();
    }

    /// <inheritdoc/>
    [PublicAPI]
    public IReadOnlyCollection<BaseCommandMetadata> RegisteredCommands => this._registeredCommands;

    /// <inheritdoc/>
    [PublicAPI]
    public IReadOnlyDictionary<HandlerKey, ICommandMetadataReceiver> Receivers => this._receivers;

    /// <inheritdoc/>
    [PublicAPI]
    public ICommandParser CommandParser { get; }

    /// <inheritdoc/>
    [PublicAPI]
    public event AsyncEventHandler<IOtyCommandsExtension, CommandHandledEventArgs> CommandHandled
    {
        add => this._commandHandledEvent!.Register(value);
        remove => this._commandHandledEvent!.Unregister(value);
    }

    /// <inheritdoc/>
    [PublicAPI]
    public event AsyncEventHandler<IOtyCommandsExtension, AddedCommandEventArgs> CommandAdded
    {
        add => this._addedCommandEvent!.Register(value);
        remove => this._addedCommandEvent!.Unregister(value);
    }

    /// <inheritdoc/>
    [PublicAPI]
    public event AsyncEventHandler<IOtyCommandsExtension, UpdatedCommandEventArgs> CommandUpdated
    {
        add => this._updatedCommandEvent!.Register(value);
        remove => this._updatedCommandEvent!.Unregister(value);
    }

    /// <inheritdoc/>
    [PublicAPI]
    public event AsyncEventHandler<IOtyCommandsExtension, RemovedCommandEventArgs> CommandRemoved
    {
        add => this._removedCommandEvent!.Register(value);
        remove => this._removedCommandEvent!.Register(value);
    }

    /// <inheritdoc/>
    [PublicAPI]
    public bool AddReceiver<TEvent, TCommand, TContext>(ICommandMetadataReceiver<TEvent, TCommand, TContext> receiver)
        where TEvent : DiscordEventArgs
        where TCommand : BaseCommandMetadata
        where TContext : BaseCommandContext
    {
        DisposedExceptionHelper.ThrowIfDisposed(this._disposed, nameof(OtyCommandsExtension));
        ArgumentNullException.ThrowIfNull(receiver, nameof(receiver));

        var handlerKey = new HandlerKey(typeof(TEvent), typeof(TCommand), typeof(TContext));

        bool isAdded = this._receivers.TryAdd(handlerKey, receiver);

        if (this._isClientReady)
        {
            this._eventRegisterer!.TryRegisterEvent(handlerKey);
        }

        return isAdded;
    }

    /// <inheritdoc/>
    [PublicAPI]
    public bool RemoveHandler(HandlerKey key)
    {
        DisposedExceptionHelper.ThrowIfDisposed(this._disposed, nameof(OtyCommandsExtension));

        if (this._isClientReady)
        {
            this._eventRegisterer!.TryUnregisterEvent(key);
        }

        return this._receivers.TryRemove(key, out _);
    }

    /// <inheritdoc/>
    [PublicAPI]
    public Task<bool> RegisterCommandAsync<TCommand>(IMetadataProvider? metadataProvider = null)
        where TCommand : BaseCommandModule, IMetadataCreatable
    {
        DisposedExceptionHelper.ThrowIfDisposed(this._disposed, nameof(OtyCommandsExtension));

        metadataProvider ??= new MetadataProvider(this._configuration.RegisteredServices);

        return this.RegisterCommandAsync(metadataProvider, TCommand.CreateMetadata);
    }

    /// <inheritdoc/>
    [PublicAPI]
    public Task<bool> RegisterCommandAsync(Type commandType, IMetadataProvider? metadataProvider = null)
    {
        DisposedExceptionHelper.ThrowIfDisposed(this._disposed, nameof(OtyCommandsExtension));
        ArgumentNullException.ThrowIfNull(commandType, nameof(commandType));

        if (!typeof(BaseCommandModule).IsAssignableFrom(commandType))
        {
            throw new ArgumentException($"Type must be inherited from {nameof(BaseCommandModule)}", nameof(commandType));
        }

        if (!typeof(IMetadataCreatable).IsAssignableFrom(commandType))
        {
            throw new ArgumentException($"Type must contain {nameof(IMetadataCreatable)}", nameof(commandType));
        }

        var metadataFunc = CreateExpression(commandType);
        metadataProvider ??= new MetadataProvider(this._configuration.RegisteredServices);

        return this.RegisterCommandAsync(metadataProvider, metadataFunc);
    }

    /// <inheritdoc/>
    [PublicAPI]
    public Task<bool> UpdateCommandAsync<TCommand>(BaseCommandMetadata oldCommand, IMetadataProvider? newMetadataProvider = null)
        where TCommand : BaseCommandModule, IMetadataCreatable
    {
        DisposedExceptionHelper.ThrowIfDisposed(this._disposed, nameof(OtyCommandsExtension));
        ArgumentNullException.ThrowIfNull(oldCommand, nameof(oldCommand));

        if (!this._registeredCommands.TryRemove(oldCommand))
        {
            return Task.FromResult(false);
        }

        newMetadataProvider ??= new MetadataProvider(this._configuration.RegisteredServices);

        return this.UpdateCommandAsync(oldCommand, newMetadataProvider, TCommand.CreateMetadata);
    }

    /// <inheritdoc/>
    [PublicAPI]
    public Task<bool> UpdateCommandAsync(BaseCommandMetadata oldCommand, Type commandType, IMetadataProvider? newMetadataProvider = null)
    {
        DisposedExceptionHelper.ThrowIfDisposed(this._disposed, nameof(OtyCommandsExtension));
        ArgumentNullException.ThrowIfNull(oldCommand, nameof(oldCommand));
        ArgumentNullException.ThrowIfNull(commandType, nameof(commandType));

        if (!typeof(BaseCommandModule).IsAssignableFrom(commandType))
        {
            throw new ArgumentException();
        }

        if (!typeof(IMetadataCreatable).IsAssignableFrom(commandType))
        {
            throw new ArgumentException($"Type must contain {nameof(IMetadataCreatable)}", nameof(commandType));
        }

        if (!this._registeredCommands.TryRemove(oldCommand))
        {
            return Task.FromResult(false);
        }

        var metadataFunc = CreateExpression(commandType);
        newMetadataProvider ??= new MetadataProvider(this._configuration.RegisteredServices);

        return this.UpdateCommandAsync(oldCommand, newMetadataProvider, metadataFunc);

    }

    /// <inheritdoc/>
    [PublicAPI]
    public async Task<bool> UnregisterCommandAsync(BaseCommandMetadata command)
    {
        DisposedExceptionHelper.ThrowIfDisposed(this._disposed, nameof(OtyCommandsExtension));
        ArgumentNullException.ThrowIfNull(command, nameof(command));

        if (!this._registeredCommands.TryRemove(command))
        {
            return false;
        }

        var removedAction = new RemovedCommandEventArgs(command);

        if (this.Client != null)
        {
            await this._removedCommandEvent!.InvokeAsync(this, removedAction, AsyncEventExceptionMode.ThrowAll);
        }

        return true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (this._disposed)
        {
            return;
        }

        this._executor.Dispose();
        this.CommandParser.Dispose();

        this._registeredCommands.Clear();
        this._receivers.Clear();

        this._commandHandledEvent?.UnregisterAll();
        this._commandHandledEvent = null;

        this._addedCommandEvent?.UnregisterAll();
        this._addedCommandEvent = null;

        this._updatedCommandEvent?.UnregisterAll();
        this._updatedCommandEvent = null;
        
        this._removedCommandEvent?.UnregisterAll();
        this._removedCommandEvent = null;

        if (this.Client is not null)
        {
            this._eventRegisterer?.Dispose();
            this._eventRegisterer = null;

            this.Client.Ready -= this.ClientReady;

            this.Client = null;
        }

        this._disposed = true;
    }

    /// <summary>
    /// Initalizes <see cref="OtyCommandsExtensionMethods"/> from given <see cref="DiscordClient"/>.
    /// <para>This method can be executed single time only.</para>
    /// </summary>
    /// <param name="client"></param>
    /// <exception cref="InvalidOperationException"></exception>
    protected override void Setup(DiscordClient client)
    {
        DisposedExceptionHelper.ThrowIfDisposed(this._disposed, nameof(OtyCommandsExtension));

        if (this.Client != null)
        {
            throw new InvalidOperationException("Cannot setup extension again.");
        }

        this.Client = client;

        this._commandHandledEvent = new("COMMAND_HANDLED", TimeSpan.Zero, null);
        this._addedCommandEvent = new("COMMAND_ADDED", TimeSpan.Zero, null);
        this._updatedCommandEvent = new("COMMAND_UPDATED", TimeSpan.Zero, null);
        this._removedCommandEvent = new("COMMAND_REMOVED", TimeSpan.Zero, null);

        this._executor.Configure(this);
        this.CommandParser.Configure(this);

        var genericMethod = this.GetType()
            .GetTypeInfo()
            .GetDeclaredMethod(nameof(this.HandleCommandHandlerAsync))!;

        this._eventRegisterer = new(client, genericMethod, this);

        client.Ready += this.ClientReady;
    }

    private Task ClientReady(DiscordClient sender, ReadyEventArgs e)
    {
        if (this._eventRegisterer is not null)
        {
            this._eventRegisterer.RegisterEvents(this._receivers.Select(c => c.Key));
        }

        this._isClientReady = true;

        return Task.CompletedTask;
    }

    [UsedImplicitly]
    private Task HandleCommandHandlerAsync<TEventArgs, TCommand, TContext>(DiscordClient sender, TEventArgs e)
        where TEventArgs : DiscordEventArgs
        where TCommand : BaseCommandMetadata
        where TContext : BaseCommandContext
    {
        var targetHandlerKey = new HandlerKey(typeof(TEventArgs), typeof(TCommand), typeof(TContext));

        if (!this._receivers.TryGetValue(targetHandlerKey, out var receiver) ||
            receiver is not ICommandMetadataReceiver<TEventArgs, TCommand, TContext> genericReceiver)
        {
            return Task.CompletedTask;
        }

        var receiverContext = new ReceiverContext<TEventArgs>(e, sender, this)
        {
            ServiceProvider = this._configuration.RegisteredServices,
        };

        if (!genericReceiver.CanExecute(receiverContext))
        {
            return Task.CompletedTask;
        }

        _ = Task.Run(async () => 
        {
            var receiverResult = await genericReceiver.GetCommandAsync(receiverContext).ConfigureAwait(false);

            if (receiverResult is null)
            {
                return;
            }

            var commandResult = await this._executor.ExecuteAsync(receiverResult).ConfigureAwait(false);

            if (!commandResult.HasValue)
            {
                return;
            }

            var commandHandledEventArgs = new CommandHandledEventArgs(this.Client, commandResult.Value);
            await this._commandHandledEvent!.InvokeAsync(this, commandHandledEventArgs, AsyncEventExceptionMode.ThrowAll).ConfigureAwait(false);

        });

        return Task.CompletedTask;
    }

    private async Task<bool> RegisterCommandAsync(IMetadataProvider metadataProvider, Func<IMetadataProvider, BaseCommandMetadata?> metadataFactory)
    {
        var commandMetadata = metadataFactory(metadataProvider);
        if (commandMetadata is null)
        {
            throw new NotImplementedException($"Class with {nameof(IMetadataCreatable)} shouldn't return null.");
        }

        if (!this._executor.CanExecute(commandMetadata, out string? reason))
        {
            throw new NotSupportedException($"{commandMetadata.Name} cannot be executed by this executor due to {reason}");
        }

        if (!this._registeredCommands.Add(commandMetadata))
        {
            return false;
        }

        var addedEventArgs = new AddedCommandEventArgs(commandMetadata, metadataProvider);

        if (this.Client != null)
        {
            await this._addedCommandEvent!.InvokeAsync(this, addedEventArgs, AsyncEventExceptionMode.ThrowAll).ConfigureAwait(false);
        }

        return true;
    }

    private async Task<bool> UpdateCommandAsync(BaseCommandMetadata oldCommand, IMetadataProvider newMetadataProvider, Func<IMetadataProvider, BaseCommandMetadata?> newMetadataFactory)
    {
        var newCommandMetadata = newMetadataFactory(newMetadataProvider);
        if (newCommandMetadata is null)
        {
            throw new InvalidOperationException($"Class with {nameof(IMetadataCreatable)} shouldn't return null.");
        }

        if (!this._executor.CanExecute(newCommandMetadata, out string? reason))
        {
            throw new NotSupportedException($"New command {newCommandMetadata.Name} is not supported bu this executor : {reason}");
        }

        if (!this._registeredCommands.Add(newCommandMetadata))
        {
            return false;
        }

        var updatedEventArgs = new UpdatedCommandEventArgs(oldCommand, newCommandMetadata, newMetadataProvider);

        if (this.Client != null)
        {
            await this._updatedCommandEvent!.InvokeAsync(this, updatedEventArgs).ConfigureAwait(false);
        }

        return true;
    }

    private static Func<IMetadataProvider, BaseCommandMetadata> CreateExpression(Type commandType)
    {
        var targetMethod = commandType.GetMethod(nameof(IMetadataCreatable.CreateMetadata), new[] { typeof(IMetadataProvider) });

        var parameter = Expression.Parameter(typeof(IMetadataProvider));
        var callExpression = Expression.Call(targetMethod!, parameter);

        return Expression.Lambda<Func<IMetadataProvider, BaseCommandMetadata>>(callExpression, parameter)
            .Compile();
    }
}