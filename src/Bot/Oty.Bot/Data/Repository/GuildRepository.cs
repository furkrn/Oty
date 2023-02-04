namespace Oty.Bot.Data.Models;

public class GuildRepository : IGuildRepository
{
    private readonly OtyDbContext _dbContext;

    public GuildRepository(OtyDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public IUnitOfWork UnitOfWork => this._dbContext;

    public async Task<Result<Guild>> HasGuildAsync(ulong guildId)
    {
        var result = await this._dbContext.Set<Guild>().FirstOrDefaultAsync(g => g.GuildId == guildId);
        return new(result is not null, result);
    }

    public async Task AddGuildAsync(Guild guild)
    {
        ArgumentNullException.ThrowIfNull(guild, nameof(guild));

        await this._dbContext.Set<Guild>()
            .AddAsync(guild);
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

    public async Task UpdateGuildAsync(Guild guild, Action<Guild> guildUpdateAction)
    {
        ArgumentNullException.ThrowIfNull(guild, nameof(guild));

        var set = this._dbContext.Set<Guild>();
        var guildModel = await set.FirstOrDefaultAsync(c => c == guild);

        if (guildModel is null)
        {
            return;
        }

        var entry = set.Attach(guild);
        guildUpdateAction(guild);

        if (entry.State is EntityState.Modified)
        {
            await entry.ReloadAsync();
        }
    }

    public async Task RemoveGuildAsync(Guild guild)
    {
        ArgumentNullException.ThrowIfNull(guild, nameof(guild));

        await Task.Yield();
        this._dbContext.Set<Guild>()
            .Remove(guild);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this._dbContext.Dispose();
        }
    }

    protected virtual ValueTask DisposeAsyncCore()
    {
        return this._dbContext.DisposeAsync();
    }
}