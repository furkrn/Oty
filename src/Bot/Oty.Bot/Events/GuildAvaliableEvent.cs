namespace Oty.Bot.Events;

[EventName(nameof(DiscordClient.GuildAvailable))]
public class GuildAvaliableEvent : IAsyncEventHandler<DiscordClient, GuildCreateEventArgs>
{
    private readonly IAsyncEventHandler<DiscordClient, GuildCreateEventArgs> _guildCreatedEventArgs;

    private readonly IOptionsSnapshot<BotConfiguration> _optionsSnapshot;

    private readonly IServiceScopeFactory _scopeFactory;

    public GuildAvaliableEvent(IStringLocalizer<GuildCreatedEvent> localizer, IStringLocalizer<GuildRequirements> requirementsLocalizer,
        IOptionsSnapshot<BotConfiguration> snapshot, IServiceScopeFactory scopeFactory, IPoProvider poProvider)
    {
        this._guildCreatedEventArgs = new GuildCreatedEvent(localizer, requirementsLocalizer, snapshot, scopeFactory, poProvider);
        this._scopeFactory = scopeFactory;
        this._optionsSnapshot = snapshot;
    }

    public async Task ExecuteAsync(DiscordClient sender, GuildCreateEventArgs e)
    {
        if (e.Guild.Id == this._optionsSnapshot.Value.SpecialGuildId)
        {
            return;
        }

        await using var scope = this._scopeFactory.CreateAsyncScope();
        await using var guildRepository = scope.ServiceProvider.GetRequiredService<IGuildRepository>();

        var (guildRecord, isNotBanned) = await guildRepository.TryLiftBan(e.Guild.Id);

        if (isNotBanned)
        {
            if (guildRecord is null)
            {
                await this._guildCreatedEventArgs.ExecuteAsync(sender, e);
            }
        }
        else
        {
            await e.Guild.LeaveAsync();
        }
    }
}