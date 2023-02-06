namespace Oty.Bot.Events;

[EventName(nameof(DiscordClient.GuildDeleted))]
public class GuildLeftEvent : IAsyncEventHandler<DiscordClient, GuildDeleteEventArgs>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public GuildLeftEvent(IServiceScopeFactory scopeFactory)
    {
        this._scopeFactory = scopeFactory;
    }

    public async Task ExecuteAsync(DiscordClient sender, GuildDeleteEventArgs e)
    {
        await using var scope = this._scopeFactory.CreateAsyncScope();
        await using var repository = scope.ServiceProvider.GetRequiredService<IGuildRepository>();

        var (guildRecord, hasGuild) = await repository.HasGuildAsync(e.Guild.Id);

        if (!hasGuild || guildRecord?.GuildState is GuildStates.Restricted)
        {
            return;
        }

        await repository.UpdateGuildAsync(guildRecord!, guild =>
        {
            guild.ContainsBot = false;
        });

        await repository.UnitOfWork.SaveChangesAsync();
    }
}