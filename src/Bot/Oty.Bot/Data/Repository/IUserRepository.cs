namespace Oty.Bot.Data.Repository;

[PublicAPI]
public interface IUserRepository : IDisposable, IAsyncDisposable
{
    [PublicAPI]
    IUnitOfWork UnitOfWork { get; }

    [PublicAPI]
    Task AddUserAsync(User user);

    [PublicAPI]
    Task AddUserAsync(ulong id, UserStates state);

    [PublicAPI]
    Task UpdateUserAsync(ulong id, Action<User> action);

    [PublicAPI]
    Task RemoveUserAsync(User user);

    [PublicAPI]
    Task<User?> GetUserAsync(ulong id);
}