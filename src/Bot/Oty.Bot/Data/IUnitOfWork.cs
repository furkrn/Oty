namespace Oty.Bot.Data;

[PublicAPI]
public interface IUnitOfWork
{
    [PublicAPI]
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}