namespace Oty.Things.AsyncMonitor;

/// <summary>
/// Represents an asynchronous property monitor.
/// </summary>
[PublicAPI]
public interface IAsyncOptionsPropertyMonitor
{
    [PublicAPI]
    bool IsConfigured { set; }
}

/// <summary>
/// Represents an asynchronous property monitor that fired when <typeparamref name="TProperty"/> value. 
/// </summary>
/// <typeparam name="TOptions">The option type.</typeparam>
/// <typeparam name="TProperty">The property type.</typeparam>
[PublicAPI]
public interface IAsyncOptionsPropertyMonitor<TOptions, TProperty> : IAsyncOptionsPropertyMonitor
    where TOptions : class
{
    /// <summary>
    /// Executes the monitor task when value has been changed.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    [PublicAPI]
    Task ExecuteAsync(ComparedValue<TOptions, TProperty> values);
}