namespace Oty.Things.AsyncMonitor;

public static class OptionsWatcherServiceCollectionExtensions
{
    /// <summary>
    /// Configures asynchronous monitors services.
    /// </summary>
    /// <param name="serviceCollection">The service collection to register.</param>
    /// <returns>Instance of <see cref="AsyncMonitorRegisterHelper"/> for registering monitors.</returns>
    [PublicAPI]
    public static AsyncMonitorRegisterHelper ConfigureAsyncMonitors(this IServiceCollection serviceCollection)
    {
        ArgumentNullException.ThrowIfNull(serviceCollection);

        serviceCollection.TryAddSingleton<IAsyncOptionsPropertyHandlerFactory, AsyncOptionsPropertyHandlerFactory>();
        serviceCollection.TryAddSingleton<IAsyncMonitorStarter, AsyncMonitorStarter>();

        return new AsyncMonitorRegisterHelper(serviceCollection);
    }
}