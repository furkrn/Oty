namespace Oty.Bot.Infastructure;

public sealed class ModuleMetadataHelper
{
    public ModuleMetadataHelper(Func<IServiceProvider, IMetadataProvider>? metadataProviderFactory, IRegisteredCheck? check)
    {
        this.MetadataProviderFactory = metadataProviderFactory;
        this.Check = check;
    }

    public Func<IServiceProvider, IMetadataProvider>? MetadataProviderFactory { get; }

    public IRegisteredCheck? Check { get; }
}