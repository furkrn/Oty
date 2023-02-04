namespace Oty.Bot.Addons.CommandLib;

public interface IAddonPublisher
{
    bool CanPublish(AddonType type);

    Task<PublishInfo> PublishAsync(IOtyCommandsExtension sender, AddonMetadata addon);

    Task<PublishInfo> UpdateAsync(IOtyCommandsExtension sender, AddonMetadata addon);

    Task<PublishInfo> RemoveAsync(IOtyCommandsExtension sender, AddonMetadata addon);
}