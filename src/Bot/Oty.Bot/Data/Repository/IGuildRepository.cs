namespace Oty.Bot.Data.Repository;

public interface IGuildRepository : IAsyncDisposable, IDisposable
{
    IUnitOfWork UnitOfWork { get; }

    Task<Result<Guild>> HasGuildAsync(ulong guildId);

    Task AddGuildAsync(Guild guild);

    Task UpdateGuildAsync(Guild guild, Action<Guild> guildUpdateAction);

    Task RemoveGuildAsync(Guild state);
}