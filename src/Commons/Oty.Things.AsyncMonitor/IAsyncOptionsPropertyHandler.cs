namespace Oty.Things.AsyncMonitor;

/// <summary>
/// Represents a handler for publishing the changes of the property.
/// </summary>
[PublicAPI]
public interface IAsyncOptionsPropertyHandler : IDisposable
{
}

/// <summary>
/// Represents a handler for publishing the changes of the <typeparamref name="TProperty"/> value.
/// </summary>
/// <typeparam name="TOptions">The option type contains the property.</typeparam>
/// <typeparam name="TProperty">The property type.</typeparam>
[PublicAPI]
public interface IAsyncOptionsPropertyHandler<TOptions, TProperty> : IAsyncOptionsPropertyHandler
    where TOptions : class
{
    /// <summary>
    /// Registers an asynchronous property monitor to the handler.
    /// </summary>
    /// <param name="task">The monitor task.</param>
    /// <typeparam name="TMonitor">The monitor type.</typeparam>
    /// <returns></returns>
    [PublicAPI]
    IDisposable AddMonitor<TMonitor>(Func<ComparedValue<TOptions, TProperty>, Task> task)
        where TMonitor : IWatchedProperty<TOptions, TProperty>;
}