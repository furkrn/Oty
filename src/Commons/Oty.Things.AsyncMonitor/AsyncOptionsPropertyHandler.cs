namespace Oty.Things.AsyncMonitor;

/// <summary>
/// An asynchronous handler for firing the change of the <typeparamref name="TProperty"/> changes on the option.
/// </summary>
/// <typeparam name="TOptions"></typeparam>
/// <typeparam name="TProperty"></typeparam>
[PublicAPI]
public class AsyncOptionsPropertyHandler<TOptions, TProperty> : IAsyncOptionsPropertyHandler<TOptions, TProperty>
    where TOptions : class
{
    private readonly IDisposable _disposable;

    private readonly IEqualityComparer<TProperty> _equalityComparer;

    private readonly PropertyWatch _propertyWatch;

    private readonly Func<TOptions, TProperty> _propertyGetter;

    private readonly ConcurrentDictionary<int, Func<ComparedValue<TOptions, TProperty>, Task>> _monitors = new();

    private readonly ConcurrentQueue<TProperty> _values = new();

    private bool _disposed;

    /// <summary>
    /// Creates an instance of <see cref="AsyncOptionsPropertyHandler{TOptions, TProperty}"/>.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="propertyWatch">The target property to monitor.</param>
    /// <param name="equalityComparer">The equality comparer.</param>
    public AsyncOptionsPropertyHandler(IServiceProvider serviceProvider, PropertyWatch propertyWatch, IEqualityComparer<TProperty> equalityComparer)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));
        ArgumentNullException.ThrowIfNull(propertyWatch, nameof(propertyWatch));
        ArgumentNullException.ThrowIfNull(equalityComparer, nameof(equalityComparer));

        this._disposable = serviceProvider.GetRequiredService<IAsyncOptionsMonitor<TOptions>>()
            .OnChange(this.PublishChangesAsync);
        this._equalityComparer = equalityComparer;
        this._propertyWatch = propertyWatch;
        this._propertyGetter = GetPropertyFunc();

        Func<TOptions, TProperty> GetPropertyFunc()
        {
            var instanceParameter = Expression.Parameter(typeof(TOptions));
            var propertyCall = Expression.Property(instanceParameter, propertyWatch.PropertyInfo);

            return Expression.Lambda<Func<TOptions, TProperty>>(propertyCall, instanceParameter)
                .Compile();
        }
    }

    /// <inheritdoc/>
    public IDisposable AddMonitor<TMonitor>(Func<ComparedValue<TOptions, TProperty>, Task> task)
        where TMonitor : IWatchedProperty<TOptions, TProperty>
    {
        var builder = new PropertyWatchBuilder<TOptions, TProperty>();

        var propertyWatch = TMonitor.Build(builder);

        if (this._propertyWatch != propertyWatch)
        {
            throw new InvalidOperationException("Specified monitor is not suitable for this monitor.");
        }

        int count = this._monitors.Count;

        this._monitors.TryAdd(count, task);

        return new MonitorRemove(count, this);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(true);

        GC.SuppressFinalize(this);
    }

    internal void RemoveMonitor(int order)
    {
        this._monitors.TryRemove(order, out _);
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                this._disposable.Dispose();
            }

            this._monitors.Clear();
            this._disposed = true;
        }
    }

    private async Task PublishChangesAsync(IAsyncOptionsMonitor<TOptions> sender, AsyncEventBox<TOptions> e)
    {
        TOptions options = e;

        var newValue = this._propertyGetter(options);

        this._values.TryDequeue(out var oldValue);
        this._values.Enqueue(newValue);

        if (!this._equalityComparer.Equals(oldValue, newValue))
        {
            var comparedValue = new ComparedValue<TOptions, TProperty>(options, oldValue, newValue);

            var monitorCalls = this._monitors.Select(c => c.Value(comparedValue));

            if (monitorCalls.Any())
            {
                await Task.WhenAny(monitorCalls)
                    .ConfigureAwait(false);
            }
        }
    }

    internal sealed class MonitorRemove : IDisposable
    {
        private readonly int _order;

        private readonly AsyncOptionsPropertyHandler<TOptions, TProperty> _handler;
        
        internal MonitorRemove(int order, AsyncOptionsPropertyHandler<TOptions, TProperty> handler)
        {
            this._order = order;
            this._handler = handler;
        }

        public void Dispose()
            => this._handler.RemoveMonitor(this._order);
    }
}
