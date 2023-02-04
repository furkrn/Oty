namespace Oty.Things.Localization;

/// <summary>
/// Translation provider for <typeparamref name="T"/> type.
/// </summary>
/// <typeparam name="T">Type for getting its translations</typeparam>
public class TranslationCollection<T> : ITranslationCollection<T>
{
    private readonly ITranslationCollection _instance;

    /// <summary>
    /// Creates an instance of <see cref="TranslationCollection{T}"/>.
    /// </summary>
    /// <param name="factory">The translation collection factory.</param>
    public TranslationCollection(ITranslationCollectionFactory factory)
    {
        this._instance = factory.Create(typeof(T));
    }

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, string?>? this[string name]
        => this._instance[name];

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, string?>? this[string name, params object[] arguments]
        => this._instance[name, arguments];

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, string?>? GetAllTranslations(string name, Plural plural, TextContext textContext, params object[]? arguments)
        => this._instance.GetAllTranslations(name, plural, textContext, arguments);

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, string?>? GetAllTranslations(string textContext, Plural plural, params object[]? arguments)
        => this._instance.GetAllTranslations(textContext, plural, arguments);
}