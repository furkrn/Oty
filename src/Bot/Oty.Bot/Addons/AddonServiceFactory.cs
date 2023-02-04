namespace Oty.Bot.Addons;

public class AddonServiceFactory : IAddonServiceFactory
{
    private readonly IEnumerable<IAddonPublisher> _publisher;

    public AddonServiceFactory(IEnumerable<IAddonPublisher> publisher)
    {
        this._publisher = publisher;
    }

    public IAddonService Create(IOtyCommandsExtension extension)
    {
        return new AddonService(extension, this._publisher);
    }
}