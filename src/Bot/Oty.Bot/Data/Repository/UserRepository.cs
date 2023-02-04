namespace Oty.Bot.Data.Repository;

[PublicAPI]
public class UserRepository : IUserRepository
{
    private readonly OtyDbContext _context;

    public UserRepository(OtyDbContext context)
    {
        this._context = context;
    }

    public IUnitOfWork UnitOfWork => this._context;

    public async Task AddUserAsync(User user)
    {
        await this._context.Users.AddAsync(user);
    }

    public Task AddUserAsync(ulong id, UserStates state)
    {
        return this.AddUserAsync(new User()
        {
            Id = id,
            UserState = state,
        });
    }

    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsyncCore();

        this.Dispose(false);

        GC.SuppressFinalize(this);
    }
    
    public async Task UpdateUserAsync(ulong id, Action<User> action)
    {
        var user = await this._context.Users.FirstOrDefaultAsync(c => c.Id == id);

        if (user is null)
        {
            return;
        }

        action(user);

        this._context.Users.Update(user);
    }

    public async Task RemoveUserAsync(User user)
    {
        await Task.Yield();

        this._context.Remove(user);
    }

    public Task<User?> GetUserAsync(ulong id)
    {
        return this._context.Users.FirstOrDefaultAsync(c => c.Id == id);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this._context.Dispose();
        }
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        await this._context.DisposeAsync();
    }
}