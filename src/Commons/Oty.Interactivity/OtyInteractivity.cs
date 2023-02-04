namespace Oty.Interactivity;

/// <summary>
/// Interactivity handler for Discord Interactions.
/// An instance of a can be created using <see cref="InteractivityExtensions.AddInteractivity(DiscordClient, Oty.Interactivity.OtyInteractivityConfiguration?)"></see>
/// </summary>
public sealed class OtyInteractivity : BaseExtension, IDisposable
{
    private readonly OtyInteractivityConfiguration _configuration;

    private ComponentWaiter _componentClickHandler;

    private ModalHandler _modalHandler;

    private ComponentInteractivityHandler _componentInteractivityHandler;

    internal OtyInteractivity(OtyInteractivityConfiguration configuration)
    {
        this._configuration = configuration;
    }

    public async Task<InteractivityResult<ComponentInteractionCreateEventArgs>> HandleClickRequestAsync(ClickInteractivityRequest request, TimeSpan timeSpan)
    {
        var cancellationTokenSource = new CancellationTokenSource(timeSpan);

        InteractivityResult<ComponentInteractionCreateEventArgs> result;
        try
        {
            result = await this.HandleClickRequestAsync(request, cancellationTokenSource.Token).ConfigureAwait(false);
        }
        finally
        {
            cancellationTokenSource.Dispose();
        }

        return result;
    }

    /// <summary>
    /// Handles an click-based interactivity request.
    /// </summary>
    [PublicAPI]
    public Task<InteractivityResult<ComponentInteractionCreateEventArgs>> HandleClickRequestAsync(ClickInteractivityRequest request, CancellationToken cancellationToken)
    {
        return this._componentClickHandler.HandleRequestAsync(request, cancellationToken);
    }

    [PublicAPI]
    public async Task<InteractivityResult<ModalSubmitEventArgs>> HandleModalRequestAsync(ModalRequest request, TimeSpan timeSpan)
    {
        var cancellationTokenSource = new CancellationTokenSource(timeSpan);

        InteractivityResult<ModalSubmitEventArgs> result;
        try
        {
            result = await this.HandleModalRequestAsync(request, cancellationTokenSource.Token).ConfigureAwait(false);
        }
        finally
        {
            cancellationTokenSource.Dispose();
        }

        return result;
    }

    [PublicAPI]
    public Task<InteractivityResult<ModalSubmitEventArgs>> HandleModalRequestAsync(ModalRequest request, CancellationToken cancellationToken)
    {
        return this._modalHandler.HandleRequestAsync(request, cancellationToken);
    }

    [PublicAPI]
    public async Task<InteractivityResult<ComponentInteractionCreateEventArgs>> HandleComponentInteraction(ComponentInteractivityRequest request, TimeSpan timeSpan)
    {
        var cancellationTokenSource = new CancellationTokenSource();

        InteractivityResult<ComponentInteractionCreateEventArgs> result;

        try
        {
            result = await this.HandleComponentInteraction(request, cancellationTokenSource.Token).ConfigureAwait(false);
        }
        finally
        {
            cancellationTokenSource.Dispose();
        }

        return result;
    }

    
    [PublicAPI]
    public Task<InteractivityResult<ComponentInteractionCreateEventArgs>> HandleComponentInteraction(ComponentInteractivityRequest request, CancellationToken cancellationToken)
    {
        return this._componentInteractivityHandler.HandleRequestAsync(request, cancellationToken);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this._componentClickHandler.Dispose();
        this._modalHandler.Dispose();
        this._componentInteractivityHandler.Dispose();
    }

    /// <summary>
    /// Configures this handler.
    /// <para>Not recommended to call this method, It can be called for a single time and it's called automaticly</para>
    /// </summary>
    /// <param name="client"></param>
    /// <exception cref="ArgumentException"></exception>
    protected override void Setup(DiscordClient client)
    {
        if (this.Client != null)
        {
            throw new ArgumentException("This method can be run for a single time!");
        }

        this.Client = client;

        this._componentClickHandler = new(client, this._configuration);
        this._modalHandler = new(client);
        this._componentInteractivityHandler = new(client);
    }
}