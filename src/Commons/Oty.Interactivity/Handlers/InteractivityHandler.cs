namespace Oty.Interactivity.Handlers;

internal abstract class InteractivityHandler<TRequest, TEvent>
    where TRequest : InteractivityRequest
    where TEvent : DiscordEventArgs
{
    protected DiscordClient Client { get; }

    protected ConcurrentDictionary<TRequest, InteractivityInteractions> Requests { get; } = new();

    protected InteractivityHandler(DiscordClient client)
    {
        this.Client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public virtual Task<InteractivityResult<TEvent>> HandleRequestAsync(TRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        return this.HandleRequestAsyncImpl(request, cancellationToken);
    }

    private async Task<InteractivityResult<TEvent>> HandleRequestAsyncImpl(TRequest request, CancellationToken cancellationToken)
    {
        var interactivityInteraction = new InteractivityInteractions();

        this.Requests.TryAdd(request, interactivityInteraction);

        var cancellationTokenRegistration = cancellationToken.Register(() => request.TimedOutTaskCompletionSource.SetResult(true));

        bool timedOut;
        try
        {
            timedOut = await request.TimedOutTaskCompletionSource.Task.ConfigureAwait(false);
        }
        finally
        {
            this.Requests.TryRemove(request, out _);

            await cancellationTokenRegistration.DisposeAsync().ConfigureAwait(false);
        }

        return new InteractivityResult<TEvent>((TEvent)interactivityInteraction.DiscordEvent, timedOut, interactivityInteraction);
    }
}