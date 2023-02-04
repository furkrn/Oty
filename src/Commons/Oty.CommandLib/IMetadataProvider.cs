namespace Oty.CommandLib;

/// <summary>
/// Provides registered services or configurations for registering commands to the extension.
/// </summary>
public interface IMetadataProvider
{
    /// <summary>
    /// Gets the registered services.
    /// </summary>
    IServiceProvider? Services { get; }
}