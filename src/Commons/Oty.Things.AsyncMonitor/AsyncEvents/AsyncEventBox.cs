namespace Oty.Things.AsyncMonitor.AsyncEvents;

/// <summary>
/// Boxes the type <typeparamref name="T"/> for making it to be used for <see cref="AsyncEvent{TSender, TArgs}"/>'s argument.
/// </summary>
[PublicAPI]
public readonly struct AsyncEventBox<T> : IAsyncEventArgs
{
    /// <summary>
    /// Creates an instance of <see cref="AsyncEventBox{T}"/>.
    /// </summary>
    /// <param name="args">The <typeparamref name="T"/> instance.</param>
    public AsyncEventBox(T args)
    {
        this.Value = args ?? throw new ArgumentNullException(nameof(args));
    }

    /// <summary>
    /// Gets the boxed value.
    /// </summary>
    /// <value></value>
    [PublicAPI]
    public T Value { get; }

    public static implicit operator T(AsyncEventBox<T> args)
        => args.Value;

    public static implicit operator AsyncEventBox<T>(T obj)
        => new(obj);
}