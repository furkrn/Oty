namespace Oty.Things.AsyncMonitor;

/// <summary>
/// Represents a asynchronous notifier used for an option.
/// </summary>
[PublicAPI]
public interface IAsyncOptionsMonitor
{
    /// <summary>
    /// Waits for option changes until <paramref name="cancellationToken"/> is cancelled.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token of the wait.</param>
    Task WaitForChangesAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a asynchronous notifier used for the <typeparamref name="TOptions"/> option type changes.
/// </summary>
/// <typeparam name="TOptions">The option type.</typeparam>
[PublicAPI]
public interface IAsyncOptionsMonitor<TOptions> : IAsyncOptionsMonitor, IDisposable
    where TOptions : class
{
    /// <summary>
    /// Gets the current value of the <typeparamref name="TOptions"/> option.
    /// </summary>
    [PublicAPI]
    TOptions CurrentValue { get; }

    /// <summary>
    /// Registers asynchronous task that fires when option <typeparamref name="TOptions"/> has changes.
    /// </summary>
    /// <param name="eventHandler"></param>
    /// <returns>Returns disposable for unregistering monitor.</returns>
    [PublicAPI]
    IDisposable OnChange(AsyncEventHandler<IAsyncOptionsMonitor<TOptions>, AsyncEventBox<TOptions>> eventHandler);
}