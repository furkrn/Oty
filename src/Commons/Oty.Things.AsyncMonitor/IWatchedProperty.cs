namespace Oty.Things.AsyncMonitor;

/// <summary>
/// Defines the specified monitor watches an property.
/// </summary>
/// <typeparam name="TOptions"></typeparam>
/// <typeparam name="TProperty"></typeparam>
[PublicAPI]
public interface IWatchedProperty<TOptions, TProperty>
    where TOptions : class
{
    /// <summary>
    /// Gets the target watched property.
    /// </summary>
    /// <param name="builder">The builder class for creating instance of it.</param>
    /// <returns></returns>
    [PublicAPI]
    static abstract PropertyWatch Build(PropertyWatchBuilder<TOptions, TProperty> builder); 
}