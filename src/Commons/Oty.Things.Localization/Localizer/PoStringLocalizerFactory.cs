namespace Oty.Things.Localization;

/// <summary>
/// Provides string localizer factory for PO String localizers
/// </summary>
[PublicAPI]
public class PoStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly IPoProvider _localizer;

    private readonly ILoggerFactory _logger;

    private readonly ConcurrentDictionary<string, PoStringLocalizer> _cachedLocalizers = new();

    public PoStringLocalizerFactory(IPoProvider localizer, ILoggerFactory loggerFactory)
    {
        this._localizer = localizer;
        this._logger = loggerFactory;
    }

    /// <inheritdoc/>
    public IStringLocalizer Create(Type resourceSource)
    {
        return Create(null, resourceSource.FullName!);
    }

    /// <inheritdoc/>
    public IStringLocalizer Create(string? baseName, string location)
    {
        return this._cachedLocalizers.GetOrAdd(location, t => new PoStringLocalizer(this._localizer, this._logger, t));
    }
}