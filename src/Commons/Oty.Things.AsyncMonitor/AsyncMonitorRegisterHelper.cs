namespace Oty.Things.AsyncMonitor;

/// <summary>
/// Helper class for registering the specified monitors to the service collection.
/// </summary>
[PublicAPI]
public sealed class AsyncMonitorRegisterHelper
{
    private readonly List<Type> _registeredOptionTypes = new();

    /// <summary>
    /// Creates an instance of <see cref="AsyncMonitorRegisterHelper"/>.
    /// </summary>
    /// <param name="serviceCollection">The service collection to register monitors.</param>
    public AsyncMonitorRegisterHelper(IServiceCollection serviceCollection)
    {
        this.ServiceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
    }

    /// <summary>
    /// Gets the service collection.
    /// </summary>
    [PublicAPI]
    public IServiceCollection ServiceCollection { get; }

    /// <summary>
    /// Registers an asynchronous property watcher using a factory.
    /// </summary>
    /// <param name="monitorFactory">The factory to create instance of the monitor.</param>
    /// <param name="lifetime">The service lifetime of the monitor.</param>
    /// <typeparam name="TOptions">The options type.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <typeparam name="TMonitor">The monitor type.</typeparam>
    [PublicAPI]
    public AsyncMonitorRegisterHelper AddPropertyMonitor<TOptions, TProperty, TMonitor>(Func<IServiceProvider, TMonitor> monitorFactory, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TOptions : class
        where TMonitor : class, IAsyncOptionsPropertyMonitor<TOptions, TProperty>, IWatchedProperty<TOptions, TProperty>
    {
        ArgumentNullException.ThrowIfNull(monitorFactory, nameof(monitorFactory));

        _ = lifetime switch
        {
            ServiceLifetime.Singleton => this.ServiceCollection.AddSingleton<IAsyncOptionsPropertyMonitor, TMonitor>(monitorFactory),
            ServiceLifetime.Scoped => this.ServiceCollection.AddScoped<IAsyncOptionsPropertyMonitor, TMonitor>(monitorFactory),
            _ => this.ServiceCollection.AddTransient<IAsyncOptionsPropertyMonitor, TMonitor>(monitorFactory),
        };

        this.AddOption<TOptions>();

        return this;
    }

    /// <summary>
    /// Registers an asynchronous property monitor.
    /// </summary>
    /// <param name="monitor">The singleton instance of the monitor.</param>
    /// <param name="lifetime">The service lifetime of the monitor.</param>
    /// <typeparam name="TOptions">The options type.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <typeparam name="TMonitor">The monitor type.</typeparam>
    [PublicAPI]
    public AsyncMonitorRegisterHelper AddPropertyMonitor<TOptions, TProperty, TMonitor>(TMonitor? monitor = null, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TOptions : class
        where TMonitor : class, IAsyncOptionsPropertyMonitor<TOptions, TProperty>, IWatchedProperty<TOptions, TProperty>
    {
        _ = lifetime switch
        {
            ServiceLifetime.Singleton when monitor is not null => this.ServiceCollection.AddSingleton(typeof(IAsyncOptionsPropertyMonitor), monitor),
            ServiceLifetime.Singleton when monitor is null => this.ServiceCollection.AddSingleton<IAsyncOptionsPropertyMonitor, TMonitor>(),
            ServiceLifetime.Scoped => this.ServiceCollection.AddScoped<IAsyncOptionsPropertyMonitor, TMonitor>(),
            _ => this.ServiceCollection.AddTransient<IAsyncOptionsPropertyMonitor, TMonitor>(),
        };

        this.AddOption<TOptions>();
        
        return this;
    }

    /// <summary>
    /// Registers a asynchronous options monitor.
    /// </summary>
    /// <typeparam name="TOptions">The option type.</typeparam>
    [PublicAPI]
    public AsyncMonitorRegisterHelper AddAsyncOptionsMonitor<TOptions>()
        where TOptions : class
    {
        this.AddOption<TOptions>();

        return this;
    }

    private void AddOption<TOptions>()
        where TOptions : class
    {
        var type = typeof(TOptions);
        if (!this._registeredOptionTypes.Contains(type))
        {
            this.ServiceCollection.AddSingleton<IAsyncOptionsMonitor<TOptions>, AsyncOptionsMonitor<TOptions>>();
            this.ServiceCollection.AddSingleton<IAsyncOptionsMonitor, IAsyncOptionsMonitor<TOptions>>(c => c.GetRequiredService<IAsyncOptionsMonitor<TOptions>>());

            this._registeredOptionTypes.Add(type);
        }
    }
}