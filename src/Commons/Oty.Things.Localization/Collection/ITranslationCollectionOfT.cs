namespace Oty.Things.Localization;

/// <summary>
/// Provides translation for the <typeparamref name="T"/> type.
/// </summary>
/// <typeparam name="T">Type to get translation for.</typeparam>
[PublicAPI]
public interface ITranslationCollection<[UsedImplicitly] T> : ITranslationCollection
{
}