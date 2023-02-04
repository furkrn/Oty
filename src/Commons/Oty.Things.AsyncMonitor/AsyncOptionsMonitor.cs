namespace Oty.Things.AsyncMonitor;

/// <summary>
/// An asynchronous monitor for monitoring the changes of <typeparamref name="TOptions"/>.
/// </summary>
/// <typeparam name="TOptions">The option type.</typeparam>
[PublicAPI]
public class AsyncOptionsMonitor<TOptions> : IAsyncOptionsMonitor<TOptions>
    where TOptions : class
{
    private readonly IOptionsMonitor<TOptions> _monitor;

    private readonly IDisposable _monitorWatchEvent;

    private readonly AsyncEvent<IAsyncOptionsMonitor<TOptions>, AsyncEventBox<TOptions>> _asyncEventHandler = new();

    private bool _disposed;
    
    private volatile TaskCompletionSource<Cancelled<TOptions>> _tcs = new();

    /// <summary>
    /// Creates an instance of <see cref="AsyncOptionsMonitor{TOptions}"/>.
    /// </summary>
    /// <param name="monitor">The monitor of the option.</param>
    public AsyncOptionsMonitor(IOptionsMonitor<TOptions> monitor)
    {
        this._monitor = monitor;
        this._monitorWatchEvent = this._monitor.OnChange(this.ChangedValue)!;
    }

    /// <inheritdoc/>
    public TOptions CurrentValue => this._monitor.CurrentValue;

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(true);

        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public IDisposable OnChange(AsyncEventHandler<IAsyncOptionsMonitor<TOptions>, AsyncEventBox<TOptions>> eventHandler)
    {
        ArgumentNullException.ThrowIfNull(eventHandler, nameof(eventHandler));

        this._asyncEventHandler.RegisterEvent(eventHandler);
        return new ChangeEvent(this._asyncEventHandler, eventHandler);
    }

    /// <inheritdoc/>
    public async Task WaitForChangesAsync(CancellationToken cancellationToken = default)
    {
        var registration = cancellationToken.Register(() => this._tcs.SetResult(new Cancelled<TOptions>(true, null)));

        try
        {
            while (true)
            {
                var tcs = this._tcs;
                var cancelledValue = await tcs.Task;

                if (cancelledValue.IsCancelled)
                {
                    break;
                }

                await this._asyncEventHandler.InvokeAsync(this, new(cancelledValue!), cancellationToken)
                    .ConfigureAwait(false);

                this._tcs = new();
            }
        }
        finally
        {
            await registration.DisposeAsync().ConfigureAwait(false);
        }
    }

#nullable disable

    /// <inheritdoc cref="IDisposable.Dispose"/>
    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                this._monitorWatchEvent.Dispose();
            }

            this._asyncEventHandler.UnregisterAll();
            this._tcs = null;

            this._disposed = true;
        }
    }

#nullable enable

    private void ChangedValue(TOptions configuration, string? _)
    {
        var tcs = this._tcs;

        tcs.SetResult(configuration);
    }

    internal sealed class ChangeEvent : IDisposable
    {
        private readonly AsyncEvent<IAsyncOptionsMonitor<TOptions>, AsyncEventBox<TOptions>> _eventHandler;

        private readonly AsyncEventHandler<IAsyncOptionsMonitor<TOptions>, AsyncEventBox<TOptions>> _eventDelegate;

        internal ChangeEvent(AsyncEvent<IAsyncOptionsMonitor<TOptions>, AsyncEventBox<TOptions>> eventHandler, AsyncEventHandler<IAsyncOptionsMonitor<TOptions>, AsyncEventBox<TOptions>> eventDelegate)
        {
            this._eventHandler = eventHandler;
            this._eventDelegate = eventDelegate;
        }

        public void Dispose()
            => this._eventHandler.UnregisterEvent(this._eventDelegate);
    }

    internal readonly struct Cancelled<TValue>
    {
        public Cancelled(bool cancelled, TValue? value)
        {
            this.IsCancelled = cancelled;
            this.Value = value;
        }

        public bool IsCancelled { get; }

        public TValue? Value { get; }

        public static implicit operator TValue?(Cancelled<TValue> cancelled)
            => cancelled.Value;

        public static implicit operator Cancelled<TValue>(TValue value)
            => new Cancelled<TValue>(false, value);
    }
}