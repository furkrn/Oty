namespace Oty.Things.Localization;

/// <summary>
/// Factory that creates instances of <see cref="ITranslationCollection"/>.
/// </summary>
public class TranslationCollectionFactory : ITranslationCollectionFactory
{
    private readonly ConcurrentDictionary<string, TranslationCollection> _collectionCache = new();

    private readonly IPoProvider _poProvider;

    private readonly IEnumerable<CultureInfo> _supportedCultures;

    /// <summary>
    /// Creates an instance of <see cref="TranslationCollectionFactory"/>.
    /// </summary>
    /// <param name="poProvider">The PO file provider.</param>
    public TranslationCollectionFactory(IPoProvider poProvider)
    {
        this._poProvider = poProvider ?? throw new ArgumentNullException(nameof(poProvider));

        this._supportedCultures = poProvider.GetAllCatalogs()
            .Where(c => c.Key.IsSuccessfull)
            .Select(c => new CultureInfo(c.Key.Locale));
    }

    /// <inheritdoc/>
    public ITranslationCollection Create(Type type)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));

        return this._collectionCache.GetOrAdd(type.FullName!,
            location => new TranslationCollection(this._poProvider, location, this._supportedCultures));
    }
}