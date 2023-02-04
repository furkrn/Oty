namespace Oty.Things.AsyncMonitor;

/// <summary>
/// Provides mechanism for starting and configuring all of the monitors.
/// </summary>
[PublicAPI]
public class AsyncMonitorStarter : IAsyncMonitorStarter
{
    private readonly IEnumerable<IAsyncOptionsMonitor> _monitors;

    private readonly IEnumerable<IAsyncOptionsPropertyMonitor> _propertyMonitors;

    /// <summary>
    /// Creates an instance of <see cref="AsyncMonitorStarter"/>.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public AsyncMonitorStarter(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));

        this._monitors = serviceProvider.GetServices<IAsyncOptionsMonitor>();

        this._propertyMonitors = serviceProvider.GetServices<IAsyncOptionsPropertyMonitor>();
    }

    /// <inheritdoc/>
    public async Task StartAllAsync(CancellationToken cancellationToken = default)
    {
        var monitorTask = this._monitors
            .Select(m => m.WaitForChangesAsync(cancellationToken));

        if (!monitorTask.Any())
        {
            return;
        }

        foreach (var propertyMonitor in this._propertyMonitors)
        {
            propertyMonitor.IsConfigured = true;
        }

        await Task.WhenAny(monitorTask)
            .ConfigureAwait(false);
    }
}