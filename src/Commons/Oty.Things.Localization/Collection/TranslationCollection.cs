namespace Oty.Things.Localization;

/// <summary>
/// Translation collection getter for all specified cultural translations.
/// </summary>
public class TranslationCollection : ITranslationCollection
{
    private readonly IPoProvider _provider;

    private readonly string _location;

    private readonly IEnumerable<CultureInfo> _supportedCultures;

    /// <summary>
    /// Creates an instance of <see cref="TranslationCollection"/>.
    /// </summary>
    /// <param name="provider">The PO file provider.</param>
    /// <param name="location">The fullname of the type that has locations.</param>
    /// <param name="supportedCultures">The supported cultures to get.</param>
    public TranslationCollection(IPoProvider provider, string location, IEnumerable<CultureInfo> supportedCultures)
    {
        ArgumentNullException.ThrowIfNull(provider, nameof(provider));
        ArgumentNullException.ThrowIfNullOrEmpty(location, nameof(location));
        ArgumentNullException.ThrowIfNull(supportedCultures, nameof(supportedCultures));

        this._provider = provider;
        this._location = location;
        this._supportedCultures = supportedCultures;
    }

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, string?> this[string id]
    {
        get
        {
            ArgumentNullException.ThrowIfNullOrEmpty(id, nameof(id));

            return this.GetAllTranslations(id: id, default, default);
        }
    }

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, string?> this[string id, params object[] arguments]
    {
        get
        {
            ArgumentNullException.ThrowIfNullOrEmpty(id, nameof(id));

            var splittedArguments = arguments.GetSpecialArguments(out var type, out var plural, out var textContext);

            return type is TranslationFindingType.WithSpecifyingEverything 
                ? this.GetAllTranslations(id, plural ?? default, textContext ?? default, splittedArguments)
                : this.GetAllTranslations(id, plural ?? default, splittedArguments);
        }
    }

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, string?> GetAllTranslations(string id, Plural plural, TextContext textContext, params object[]? arguments)
    {
        return this.GetAllTranslations(_ => new POKey(id, plural.Id, textContext.Id), plural.Count, arguments);
    }

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, string?> GetAllTranslations(string textContext, Plural plural, params object[]? arguments)
    {
        return this.GetAllTranslations(c => c.Keys.FirstOrDefault(k => k.ContextId == textContext && k.PluralId == plural.Id),
            plural.Count, arguments);
    }

    private IReadOnlyDictionary<string, string?> GetAllTranslations(Func<POCatalog, POKey?> keyFunc, int pluralCount, params object[]? arguments)
    {
        var dictionary = new Dictionary<string, string?>();

        foreach (var culture in this._supportedCultures)
        {
            var (catalog, location) = this._provider.GetCatalog(culture);

            if (!location.IsSuccessfull)
            {
                continue;
            }

            var poKey = keyFunc(catalog!);

            if (poKey is null)
            {
                continue;
            }

            if (!catalog!.TryGetValue(poKey.Value, out var poEntry) || !IsReferenced(poEntry))
            {
                continue;
            }

            string translation = arguments?.Length > 0
                ? string.Format(catalog.GetTranslation(poKey.Value, pluralCount), arguments)
                : catalog.GetTranslation(poKey.Value, pluralCount);

            if (string.IsNullOrWhiteSpace(translation))
            {
                continue;
            }

            dictionary.Add(culture.Name, translation);

            bool IsReferenced(IPOEntry entry)
            {
                var referenceComment = entry.Comments.OfType<POTranslatorComment>()
                .FirstOrDefault(k => k.Text == this._location);

                return referenceComment is not null;
            }
        }

        return dictionary;
    }
}