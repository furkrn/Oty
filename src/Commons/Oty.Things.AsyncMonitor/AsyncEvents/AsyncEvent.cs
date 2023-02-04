namespace Oty.Things.AsyncMonitor.AsyncEvents;

/// <summary>
/// An asynchronous event handler.
/// </summary>
/// <typeparam name="TSender">The sender of the event.</typeparam>
/// <typeparam name="TArgs">The argument of the event.</typeparam>
[PublicAPI]
public class AsyncEvent<TSender, TArgs>
    where TArgs : IAsyncEventArgs
{
    private readonly object _locker = new();
    
    private ImmutableArray<AsyncEventHandler<TSender, TArgs>> _handlers = ImmutableArray<AsyncEventHandler<TSender, TArgs>>.Empty;

    /// <summary>
    /// Registers a handler to the event.
    /// </summary>
    /// <param name="handler">The asynchronous task to register.</param>
    [PublicAPI]
    public void RegisterEvent(AsyncEventHandler<TSender, TArgs> handler)
    {
        ArgumentNullException.ThrowIfNull(handler, nameof(handler));

        lock (this._locker)
        {
            this._handlers = this._handlers.Add(handler);
        }
    }

    /// <summary>
    /// Unregisters a handler from the event. 
    /// </summary>
    /// <param name="handler">The asynchronous task to unregister.</param>
    [PublicAPI]
    public void UnregisterEvent(AsyncEventHandler<TSender, TArgs> handler)
    {
        ArgumentNullException.ThrowIfNull(handler, nameof(handler));

        lock (this._locker)
        {
            this._handlers = this._handlers.Remove(handler);
        }
    }

    /// <summary>
    /// Unregisters all of the handler from the event.
    /// </summary>
    [PublicAPI]
    public void UnregisterAll()
    {
        lock (this._locker)
        {
            this._handlers = this._handlers.Clear();
        }
    }

    /// <summary>
    /// Invokes all of the registered handlers to this event.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The arguments of the event.</param>
    /// <param name="cancellationToken">The cancellation token for cancelling the invokation of the handlers.</param>
    [PublicAPI]
    public Task InvokeAsync(TSender sender, TArgs args, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sender, nameof(sender));
        ArgumentNullException.ThrowIfNull(args, nameof(args));

        return this.InvokeAsyncImpl(sender, args, cancellationToken);
    }

    private async Task InvokeAsyncImpl(TSender sender, TArgs args, CancellationToken cancellationToken)
    {
        // TODO : https://www.jetbrains.com/resharperplatform/help?Keyword=InconsistentlySynchronizedField

        if (this._handlers.Length is 0)
        {
            return;
        }

        var taskArray = new Task[this._handlers.Length];

        for (int i = 0; i < taskArray.Length; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            taskArray[i] = this._handlers[i](sender, args);
        }

        await Task.WhenAny(taskArray);
    }

    public static AsyncEvent<TSender, TArgs> operator +(AsyncEvent<TSender, TArgs> asyncEvent, AsyncEventHandler<TSender, TArgs> handler)
    {
        asyncEvent.RegisterEvent(handler);

        return asyncEvent;
    }

    public static AsyncEvent<TSender, TArgs> operator -(AsyncEvent<TSender, TArgs> asyncEvent, AsyncEventHandler<TSender, TArgs> handler)
    {
        asyncEvent.UnregisterEvent(handler);

        return asyncEvent;
    }
}