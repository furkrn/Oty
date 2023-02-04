namespace Oty.Things.AsyncMonitor;

/// <summary>
/// The factory for creating instance of <see cref="IAsyncOptionsPropertyHandler{TOptions, TProperty}"/>.
/// </summary>
[PublicAPI]
public class AsyncOptionsPropertyHandlerFactory : IAsyncOptionsPropertyHandlerFactory
{
    private readonly ConcurrentDictionary<PropertyWatch, IAsyncOptionsPropertyHandler> _handlerCollection = new();

    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Creates an instance of <see cref="AsyncOptionsPropertyHandlerFactory"/>.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public AsyncOptionsPropertyHandlerFactory(IServiceProvider serviceProvider)
    {
        this._serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc/>
    public IAsyncOptionsPropertyHandler<TOptions, TProperty> CreateFrom<TOptions, TProperty>(PropertyWatch propertyWatch, IEqualityComparer<TProperty>? equalityComparer = null)
        where TOptions : class
    {
        ArgumentNullException.ThrowIfNull(propertyWatch, nameof(propertyWatch));

        return (IAsyncOptionsPropertyHandler<TOptions, TProperty>)this._handlerCollection.GetOrAdd(propertyWatch,
            p => new AsyncOptionsPropertyHandler<TOptions, TProperty>(this._serviceProvider, p, equalityComparer ?? EqualityComparer<TProperty>.Default));
    }
}