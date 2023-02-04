namespace Oty.Things.AsyncMonitor;

/// <summary>
/// Base class that represents an asynchronous property monitor fires when <typeparamref name="TProperty"/> has changed.
/// </summary>
/// <typeparam name="TOptions">The option type.</typeparam>
/// <typeparam name="TProperty">the property type.</typeparam>
/// <typeparam name="TSelf">The monitor type.</typeparam>
[PublicAPI]
public abstract class BaseAsyncOptionsPropertyMonitor<TOptions, TProperty, TSelf> : IAsyncOptionsPropertyMonitor<TOptions, TProperty>
    where TOptions : class
    where TSelf : BaseAsyncOptionsPropertyMonitor<TOptions, TProperty, TSelf>, IWatchedProperty<TOptions, TProperty>
{
    private readonly IDisposable _disposable;

    private bool _disposed;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="factory"></param>
    [PublicAPI]
    protected BaseAsyncOptionsPropertyMonitor(IAsyncOptionsPropertyHandlerFactory factory)
    {
        var builder = new PropertyWatchBuilder<TOptions, TProperty>();

        this._disposable = factory.CreateFrom<TOptions, TProperty>(TSelf.Build(builder))
            .AddMonitor<TSelf>(this.ExecuteAsync);
    }

    /// <summary>
    /// Gets whether monitor is configured.
    /// </summary>
    /// <value></value>
    [PublicAPI]
    public bool IsConfigured { protected get; set; }

    /// <inheritdoc/>
    [PublicAPI]
    public void Dispose()
    {
        this.Dispose(true);

        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    [PublicAPI]
    public abstract Task ExecuteAsync(ComparedValue<TOptions, TProperty> values);

    /// <inheritdoc cref="IDisposable.Dispose()"/>
    [PublicAPI]
    protected virtual void Dispose(bool disposing)
    {
        if (this._disposed)
        {
            if (disposing)
            {
                this._disposable.Dispose();
            }

            this._disposed = true;
        }
    }
}