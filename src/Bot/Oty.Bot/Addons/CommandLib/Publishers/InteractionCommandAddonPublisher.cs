namespace Oty.Bot.Addons.CommandLib.Publishers;

public sealed class InteractionCommandAddonPublisher : IAddonPublisher
{
    public bool CanPublish(AddonType type)
    {
        return type is not AddonType.AutoComplete;
    }

    public Task<PublishInfo> PublishAsync(IOtyCommandsExtension sender, AddonMetadata addon)
    {
        throw new NotImplementedException();
    }

    public Task<PublishInfo> UpdateAsync(IOtyCommandsExtension sender, AddonMetadata addon)
    {
        throw new NotImplementedException();
    }

    public Task<PublishInfo> RemoveAsync(IOtyCommandsExtension sender, AddonMetadata addon)
    {
        throw new NotImplementedException();
    }
}