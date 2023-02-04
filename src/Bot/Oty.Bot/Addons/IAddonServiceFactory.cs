namespace Oty.Bot.Addons;

public interface IAddonServiceFactory
{
    IAddonService Create(IOtyCommandsExtension extension);
}