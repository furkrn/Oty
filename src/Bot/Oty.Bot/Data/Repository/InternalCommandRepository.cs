namespace Oty.Bot.Data.Repository;

[PublicAPI]
public class InternalCommandRepository : IInternalCommandRepository
{
    private readonly OtyDbContext _dbContext;

    public InternalCommandRepository(OtyDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public IUnitOfWork UnitOfWork => this._dbContext;

    public Task<bool> AnyAsync(Expression<Func<InternalCommand, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

        return this._dbContext.Set<InternalCommand>()
            .AnyAsync(predicate);
    }

    public async Task<Result<InternalCommand>> TryAdd(InternalCommand command)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));

        var set = this._dbContext.Set<InternalCommand>();
        if (await set.AnyAsync(c => c == command))
        {
            return default;
        }

        await set.AddAsync(command).ConfigureAwait(false);

        return new Result<InternalCommand>(true, command);
    }

    public Task<Result<InternalCommand>> TryAdd(IDiscordPublishable publishable)
    {
        ArgumentNullException.ThrowIfNull(publishable, nameof(publishable));

        return this.TryAdd(CreateFromPublishable(publishable));
    }

    public async Task<bool> ContainsAsync(InternalCommand command)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));

        return await this._dbContext.Set<InternalCommand>()
            .ContainsAsync(command);
    }

    public Task<bool> ContainsAsync(IDiscordPublishable publishable)
    {
        ArgumentNullException.ThrowIfNull(publishable, nameof(publishable));
        
        return this.ContainsAsync(CreateFromPublishable(publishable));
    }

    public void Dispose()
    {
        this.Dispose(true);

        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsyncCore();

        this.Dispose(false);

        GC.SuppressFinalize(this);
    }

    public Task<InternalCommand?> GetAsync(ulong id)
    {
        return this._dbContext.Set<InternalCommand>()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public Task<InternalCommand?> GetAsync(string name, ApplicationCommandType type)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(name, nameof(name));

        return this._dbContext.Set<InternalCommand>()
            .FirstOrDefaultAsync(c => c.Name == name && c.CommandType == type);
    }

    public IEnumerable<InternalCommand> GetCommands()
    {
        return this._dbContext.Set<InternalCommand>();
    }

    public Task<int> GetCountAsync(Expression<Func<InternalCommand, bool>>? predicate = null)
    {
        var set = this._dbContext.Set<InternalCommand>();

        return predicate is null
            ? set.CountAsync()
            : set.CountAsync(predicate);
    }

    public async Task<Result<InternalCommand>> UpdateAsync(ulong id, Func<InternalCommand, InternalCommand> updateFactory)
    {
        ArgumentNullException.ThrowIfNull(updateFactory, nameof(updateFactory));

        var command = await this._dbContext.Set<InternalCommand>()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (command is null)
        {
            return default;
        }

        return await this.UpdateAsync(command, updateFactory);
    }

    // resharper disable AccessToStaticMemberViaDerivedType

    public async Task<Result<InternalCommand>> UpdateAsync(string name, ApplicationCommandType type, Func<InternalCommand, InternalCommand> updateFactory)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(name, nameof(name));
        ArgumentNullException.ThrowIfNull(updateFactory, nameof(updateFactory));

        var command = await this._dbContext.Set<InternalCommand>()
            .FirstOrDefaultAsync(c => c.Name == name && c.CommandType == type);

        if (command is null)
        {
            return default;
        }

        return await this.UpdateAsync(command, updateFactory);
    }

    public async Task BulkInsertOrUpdateOrDeleteFromAsync(IEnumerable<IDiscordPublishable> publishables)
    {
        ArgumentNullException.ThrowIfNull(publishables, nameof(publishables));

        var set = this._dbContext.Set<InternalCommand>();
        var trackingSet = set.AsTracking();

        foreach (var publishable in publishables)
        {
            var command = await trackingSet.FirstOrDefaultAsync(c => c.Name == publishable.Name && c.CommandType == publishable.CommandType)
                .ConfigureAwait(false);

            if (command is not null)
            {
                command.Id = publishable.Id;
                command.GuildId = publishable.RegisteredGuildId;
            }
            else
            {
                await set.AddAsync(CreateFromPublishable(publishable)).ConfigureAwait(false);
            }
        }

        var skippedItems = await EnumerableExtensions.SkipWhileAsync(set, e => set.ContainsAsync(e));

        if (skippedItems.Any())
        {
            set.RemoveRange(skippedItems);
        }
    }

    public IEnumerable<InternalCommand> GetCommandsBy(Func<InternalCommand, bool> func)
    {
        ArgumentNullException.ThrowIfNull(func, nameof(func));

        return this._dbContext.Set<InternalCommand>()
            .Where(func);
    }

    public async Task<Result<InternalCommand>> TryRemove(ulong id)
    {
        var set = this._dbContext.Set<InternalCommand>();
        var command = await set.FirstOrDefaultAsync(p => p.Id == id);

        if (command is null)
        {
            return default;
        }

        set.Remove(command);

        return new Result<InternalCommand>(true, command);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this._dbContext.Dispose();
        }
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        await this._dbContext.DisposeAsync();
    }

    private async Task<Result<InternalCommand>> UpdateAsync(InternalCommand command, Func<InternalCommand, InternalCommand> updateFactory)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(updateFactory, nameof(updateFactory));

        var set = this._dbContext.Set<InternalCommand>();

        var attach = set.Attach(command);
        var updatedValue = updateFactory(command);

        if (attach.State is not EntityState.Modified)
        {
            return default;
        }

        await attach.ReloadAsync();

        return new Result<InternalCommand>(true, updatedValue);
    }

    private static InternalCommand CreateFromPublishable(IDiscordPublishable publishable)
    {    
        return new InternalCommand()
        {
            Name = publishable.Name,
            CommandType = publishable.CommandType,
            Id = publishable.Id,
            GuildId = publishable.RegisteredGuildId,
            HashCode = publishable.GetHashCode(),
        };
    }
}