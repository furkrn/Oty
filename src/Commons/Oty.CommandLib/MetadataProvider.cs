namespace Oty.CommandLib;

/// <inheritdoc cref="IMetadataProvider"/>
public class MetadataProvider : IMetadataProvider
{
    public MetadataProvider(IServiceProvider? services)
    {
        this.Services = services;
    }

    /// <inheritdoc/>
    public IServiceProvider? Services { get; }
}