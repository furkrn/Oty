namespace Oty.Bot.Addons;

public interface IAddonService : IAsyncDisposable, IDisposable
{
    uint ShardId { get; }

    Task<PublishInfo> CreateAddonAsync(ulong guildId);

    Task<PublishInfo> UpdateAddonAsync(ulong guildId);

    Task<PublishInfo> RemoveAddonFrom(ulong guildId);
}