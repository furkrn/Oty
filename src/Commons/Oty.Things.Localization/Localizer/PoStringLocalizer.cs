namespace Oty.Things.Localization;

[PublicAPI]
public class PoStringLocalizer : IPoStringLocalizer
{
    private readonly IPoProvider _provider;

    private readonly ILogger<PoStringLocalizer> _logger;

    private readonly string _location;

    /// <summary>
    /// Creates a new instance of <see cref="PoStringLocalizer"/>
    /// </summary>
    /// <param name="provider">The PO file provider.</param>
    /// <param name="logger">The logging factory.</param>
    /// <param name="location">The fullname of the type that has localizations.</param>
    [PublicAPI]
    public PoStringLocalizer(IPoProvider provider, ILoggerFactory logger, string location)
    {
        this._provider = provider ?? throw new ArgumentNullException(nameof(provider));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        this._logger = logger.CreateLogger<PoStringLocalizer>();
        this._location = location;
    }

    /// <inheritdoc/>
    [PublicAPI]
    public LocalizedString this[string name]
    {
        get
        {
            ArgumentNullException.ThrowIfNullOrEmpty(name, nameof(name));

            return this.GetTranslation(id: name, default, default);
        }
    }

    /// <inhertidoc/>
    [PublicAPI]
    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            ArgumentNullException.ThrowIfNullOrEmpty(name, nameof(name));

            var splittedArguments = arguments.GetSpecialArguments(out var type, out var plural, out var textContext);

            return type is TranslationFindingType.WithSpecifyingEverything
                ? this.GetTranslation(name, plural ?? default, textContext ?? default, splittedArguments)
                : this.GetTranslation(textContext: name, plural ?? default, splittedArguments);
        }
    }

    /// <inhertidoc/>
    [PublicAPI]
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return includeParentCultures
            ? GetAllStringsWithParents()
            : GetAllStrings(CultureInfo.CurrentCulture);
    }

    /// <inhertidoc/>
    [PublicAPI]
    public LocalizedString GetTranslation(string id, Plural plural, TextContext textContext, params object[]? arguments)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(id, nameof(id));

        return this.GetString(CreateKey(id, plural, textContext), arguments, plural.Count);
    }

    /// <inhertidoc/>
    [PublicAPI]
    public LocalizedString GetTranslation(string textContext, Plural plural, params object[]? arguments)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(textContext, nameof(textContext));

        var entry = this._provider.GetCatalog(CultureInfo.CurrentCulture);

        if (!entry.Location.IsSuccessfull)
        {
            return new LocalizedString(textContext, textContext, false, entry.Location.Location);
        }

        var keys = entry.Catalog!.Keys.Where(c => c.ContextId == textContext && c.PluralId == plural.Id)
            .ToArray();

        if (keys.Length != 1)
        {
            return new LocalizedString(textContext, textContext, false, entry.Location.Location);
        }

        return this.GetString(keys[0], arguments, plural.Count, entry);
    }

    /// <inhertidoc/>
    [PublicAPI]
    public bool TryGetTranslation(string id, Plural plural, TextContext textContext, [MaybeNullWhen(false)] out string? translation)
    {
        return this.TryGetTranslation(id, plural, textContext, null, out translation);
    }

    /// <inhertidoc/>
    [PublicAPI]
    public bool TryGetTranslation(string id, Plural plural, TextContext textContext, object[]? arguments, [MaybeNullWhen(false)] out string? translation)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(id, nameof(id));

        var key = CreateKey(id, plural, textContext);

        if (!this.HasTranslation(key, out var entry))
        {
            translation = null;
            return false;
        }

        var localizedString = this.GetString(key, arguments, plural.Count, entry);

        if (localizedString.ResourceNotFound)
        {
            translation = null;
            return false;
        }

        translation = localizedString.Value;
        return true;
    }

    /// <inhertidoc/>
    [PublicAPI]
    public bool TryGetTranslation(string textContext, Plural plural, object[]? arguments, [MaybeNullWhen(false)] out string? translation)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(textContext, nameof(textContext));

        var localizedString = this.GetTranslation(textContext, plural, arguments);

        translation = !localizedString.ResourceNotFound
            ? null
            : localizedString.Value;
        
        return !localizedString.ResourceNotFound;
    }

    private static POKey CreateKey(string id, Plural plural, TextContext textContext)
        => new(id, plural.Id, textContext.Id);

    private IEnumerable<LocalizedString> GetAllStrings(CultureInfo culture)
    {
        var (catalog, location) = this._provider.GetCatalog(culture);

        if (!location.IsSuccessfull)
        {
            this._logger.LogError("Cannot load translations for {0}.", culture.Name);

            yield break;
        }

        foreach (var entry in catalog!)
        {
            if (!this.IsReferenced(entry))
            {
                continue;
            }

            yield return new LocalizedString(entry.Key.Id, entry[0], false, location.Location);
        }
    }

    private IEnumerable<LocalizedString> GetAllStringsWithParents()
    {
        var currentCulture = CultureInfo.CurrentCulture;

        var localizedStringList = new List<LocalizedString>();

        do
        {
            foreach (var localizedString in this.GetAllStrings(currentCulture))
            {
                if (localizedStringList.All(ls => ls.Name == localizedString.Name))
                {
                    localizedStringList.Add(localizedString);
                }
            }

            currentCulture = currentCulture.Parent;
        }
        while(currentCulture != currentCulture.Parent);

        return localizedStringList;
    }

    private LocalizedString GetString(POKey key, object[]? arguments, int pluralCount, PoEntry? entry = null)
    {
        entry ??= this._provider.GetCatalog(CultureInfo.CurrentCulture);
        var (catalog, location) = entry.Value;

        if (catalog is null)
        {
            return new LocalizedString(key.Id, key.Id, true);
        }

        if (!catalog.TryGetValue(key, out var poEntry) || !this.IsReferenced(poEntry))
        {
            return new LocalizedString(key.Id, key.Id, true);
        }

        string value = key.PluralId == null
            ? catalog.GetTranslation(key)
            : catalog.GetTranslation(key, pluralCount);

        if (value is null)
        {
            return new LocalizedString(key.Id, key.Id, true);
        }

        return new LocalizedString(key.Id, arguments?.Length > 0 
            ? string.Format(value, arguments)
            : value, false, location.Location);
    }

    private bool HasTranslation(POKey key, out PoEntry entry)
    {
        entry = this._provider.GetCatalog(CultureInfo.CurrentCulture);

        return entry.Location.IsSuccessfull && entry.Catalog?.TryGetValue(key, out _) is true;
    }

    private bool IsReferenced(IPOEntry entry)
    {
        var referenceComment = entry.Comments.OfType<POTranslatorComment>()
            .FirstOrDefault(k => k.Text == this._location);

        return referenceComment is not null;
    }
}