namespace Oty.Bot.Addons;

public class AddonService : IAddonService
{
    private readonly IEnumerable<IAddonPublisher> _publisher;

    private readonly IOtyCommandsExtension _extension;

    public AddonService(IOtyCommandsExtension extension, IEnumerable<IAddonPublisher> publisher)
    {
        this._extension = extension;
        this._publisher = publisher;
    }

    public uint ShardId { get; private set; }

    public Task<PublishInfo> CreateAddonAsync(ulong guildId)
    {
        throw new NotImplementedException();
    }

    public Task<PublishInfo> UpdateAddonAsync(ulong guildId)
    {
        throw new NotImplementedException();
    }

    public Task<PublishInfo> RemoveAddonFrom(ulong guildId)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    private Task OnRegister(IOtyCommandsExtension sender, AddedCommandEventArgs e)
    {
        return Task.CompletedTask;
    }

    private Task OnUpdate(IOtyCommandsExtension sender, UpdatedCommandEventArgs e)
    {
        return Task.CompletedTask;
    }

    private Task OnRemove(IOtyCommandsExtension sender, RemovedCommandEventArgs e)
    {
        return Task.CompletedTask;
    }
}