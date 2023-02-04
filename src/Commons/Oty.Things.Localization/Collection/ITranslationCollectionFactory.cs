namespace Oty.Things.Localization;

/// <summary>
/// Represents a factory that creates instance of a <see cref="ITranslationCollection"/>.
/// </summary>
[PublicAPI]
public interface ITranslationCollectionFactory
{
    /// <summary>
    /// Creates an instance of a <see cref="ITranslationCollection"/>.
    /// </summary>
    /// <param name="type">Target type to get translation for.</param>
    /// <returns>An Instance of <see cref="ITranslationCollection"/>.</returns>
    [PublicAPI]
    ITranslationCollection Create(Type type);
}