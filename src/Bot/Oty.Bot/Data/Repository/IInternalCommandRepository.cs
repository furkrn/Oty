namespace Oty.Bot.Data.Repository;

[PublicAPI]
public interface IInternalCommandRepository : IDisposable, IAsyncDisposable
{
    [PublicAPI]
    IUnitOfWork UnitOfWork { get; }

    [PublicAPI]
    Task<bool> AnyAsync(Expression<Func<InternalCommand, bool>> predicate);

    [PublicAPI]
    Task<Result<InternalCommand>> TryAdd(InternalCommand command);

    [PublicAPI]
    Task<Result<InternalCommand>> TryAdd(IDiscordPublishable publishable);

    [PublicAPI]
    Task<bool> ContainsAsync(InternalCommand command);

    [PublicAPI]
    Task<bool> ContainsAsync(IDiscordPublishable publishable);

    [PublicAPI]
    Task<InternalCommand?> GetAsync(ulong id);

    [PublicAPI]
    Task<InternalCommand?> GetAsync(string name, ApplicationCommandType type);

    [PublicAPI]
    IEnumerable<InternalCommand> GetCommands();

    [PublicAPI]
    Task<int> GetCountAsync(Expression<Func<InternalCommand, bool>>? predicate = null);

    [PublicAPI]
    Task<Result<InternalCommand>> UpdateAsync(ulong id, Func<InternalCommand, InternalCommand> updateFactory);

    [PublicAPI]
    Task<Result<InternalCommand>> UpdateAsync(string name, ApplicationCommandType type, Func<InternalCommand, InternalCommand> updateFactory);

    [PublicAPI]
    Task BulkInsertOrUpdateOrDeleteFromAsync(IEnumerable<IDiscordPublishable> publishables);

    [PublicAPI]
    IEnumerable<InternalCommand> GetCommandsBy(Func<InternalCommand, bool> func);

    [PublicAPI]
    Task<Result<InternalCommand>> TryRemove(ulong id);
}