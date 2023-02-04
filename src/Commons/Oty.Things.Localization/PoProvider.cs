namespace Oty.Things.Localization;

/// <summary>
/// Parsed PO File provider.
/// </summary>
[PublicAPI]
public class PoProvider : IPoProvider
{
    private readonly PoLocalizationOptions _options;

    private readonly ILogger<PoProvider> _logger;

    private readonly Dictionary<PoLocation, POCatalog> _catalogs;

    private const string PoFileExtension = ".po";

    /// <summary>
    /// Creates am instamce of <see cref="PoProvider"/>.
    /// </summary>
    /// <param name="options">Options of the PO parser.</param>
    /// <param name="logger">Logging factory.</param>
    public PoProvider(IOptions<PoLocalizationOptions> options, ILogger<PoProvider> logger)
    {
        this._options = options.Value ?? throw new ArgumentNullException(nameof(options));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this._catalogs = this.LoadFiles();
    }

    /// <summary>
    /// Gets the all successfull catalogs.
    /// </summary>
    /// <returns></returns>
    public IReadOnlyDictionary<PoLocation, POCatalog> GetAllCatalogs()
    {
        return this._catalogs;
    }

    /// <summary>
    /// Gets a catalog based on its culture.
    /// </summary>
    /// <param name="cultureInfo">Culture of catalog.</param>
    /// <returns>Catalog with its location information.</returns>
    public PoEntry GetCatalog(CultureInfo cultureInfo)
    {
        var (poLocation, catalog) = this._catalogs.FirstOrDefault(c => c.Key.Locale == cultureInfo.Name);

        return new PoEntry(catalog, poLocation);
    }

    private Dictionary<PoLocation, POCatalog> LoadFiles()
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), this._options.ResourcesPath);

        if (!Directory.Exists(path))
        {
            throw new InvalidOperationException($"{path} does not exist.");
        }

        return Directory.GetFiles(path)
            .Where(s => Path.GetExtension(s) == PoFileExtension)
            .Select(LoadFile)
            .Where(item => item.Location.IsSuccessfull)
            .ToDictionary(c => c.Location, c => c.Catalog!);
    }

    private (PoLocation Location, POCatalog? Catalog) LoadFile(string fileName)
    {
        using var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

        var poResult = new POParser(this._options.ParserSettings)
            .Parse(fileStream);

        fileStream.Flush();
        fileStream.Close();

        if (!poResult.Success)
        {
            this._logger.LogError("Error(s) occurred for parsing {0} po file: {1}", fileName, string.Join('\n', poResult.Diagnostics.Select(c => c.ToString())));
            this._logger.LogWarning("Failed to load translation file of '{0}' Skipped translation file.", fileName);

            return (default, null);
        }

        string locale = new CultureInfo(Path.GetFileNameWithoutExtension(fileName), false)
            .Name;

        var location = new PoLocation(fileName, locale, true);

        this._logger.LogInformation("Loaded {0} sucessfully.", fileName);

        return (location, poResult.Catalog);
    }
}