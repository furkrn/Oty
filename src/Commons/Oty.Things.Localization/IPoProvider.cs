namespace Oty.Things.Localization;

/// <summary>
/// Represents Parsed PO file provider.
/// </summary>
[PublicAPI]
public interface IPoProvider
{
    /// <summary>
    /// Gets all the catalogs
    /// </summary>
    /// <returns></returns>
    [PublicAPI]
    IReadOnlyDictionary<PoLocation, POCatalog> GetAllCatalogs();

    /// <summary>
    /// Gets the catalog based from culture.
    /// </summary>
    /// <param name="cultureInfo">Culture of the catalog to get.</param>
    /// <returns>Location of the catalog and the catalog itself.</returns>
    [PublicAPI]
    PoEntry GetCatalog(CultureInfo cultureInfo);
}