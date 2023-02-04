namespace Oty.Things.AsyncMonitor;

/// <summary>
/// Represents a factory for creating instance of <see cref="IAsyncOptionsPropertyHandler{TOptions, TProperty}"/>.
/// </summary>
[PublicAPI]
public interface IAsyncOptionsPropertyHandlerFactory
{
    /// <summary>
    /// Creates an instance of <see cref="IAsyncOptionsPropertyHandler{TOptions, TProperty}"/>.
    /// </summary>
    /// <param name="propertyWatch">The information of the property to monitor.</param>
    /// <param name="equalityComparer">The equality comparer for comparing the values.</param>
    /// <typeparam name="TOptions">The options type.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <returns></returns>
    [PublicAPI]
    IAsyncOptionsPropertyHandler<TOptions, TProperty> CreateFrom<TOptions, TProperty>(PropertyWatch propertyWatch, IEqualityComparer<TProperty>? equalityComparer = null)
        where TOptions : class;
}