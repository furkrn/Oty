namespace Oty.Things.AsyncMonitor;

/// <summary>
/// Defines a mechanism for starting and configuring all of the monitors.
/// </summary>
[PublicAPI]
public interface IAsyncMonitorStarter
{
    /// <summary>
    /// Starts all the monitors.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the task.</param>
    [PublicAPI]
    Task StartAllAsync(CancellationToken cancellationToken = default);
}