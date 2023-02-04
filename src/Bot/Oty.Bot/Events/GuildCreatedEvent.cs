namespace Oty.Bot.Events;

[EventName(nameof(DiscordClient.GuildCreated))]
public class GuildCreatedEvent : IAsyncEventHandler<DiscordClient, GuildCreateEventArgs>
{
    private readonly IStringLocalizer<GuildCreatedEvent> _localizer;

    private readonly IStringLocalizer<GuildRequirements> _requirementsLocalizer;

    private readonly IOptionsSnapshot<BotConfiguration> _snapshot;

    private readonly IServiceScopeFactory _scopeFactory;

    private readonly IPoProvider _poProvider;

    public GuildCreatedEvent(IStringLocalizer<GuildCreatedEvent> localizer, IStringLocalizer<GuildRequirements> requirementsLocalizer,
        IOptionsSnapshot<BotConfiguration> snapshot, IServiceScopeFactory scopeFactory, IPoProvider poProvider)
    {
        this._localizer = localizer;
        this._requirementsLocalizer = requirementsLocalizer;
        this._snapshot = snapshot;
        this._scopeFactory = scopeFactory;
        this._poProvider = poProvider;
    }

    public async Task ExecuteAsync(DiscordClient client, GuildCreateEventArgs e)
    {
        await using var scope = this._scopeFactory.CreateAsyncScope();
        await using var guildRepository = scope.ServiceProvider.GetRequiredService<IGuildRepository>();

        var guild = e.Guild;

        CultureInfo.CurrentCulture = guild.GetGuildCultureInfo(this._poProvider);

        bool botCheck = await HasMoreMembersThanBots(guild);

        ushort minimumMemberCount = this._snapshot.Value.MinimumMemberRequirement;

        var requirementsBuilder = new GuildRequirementsBuilder(guild)
            .AddRequirement(_ => botCheck, this._localizer["Has more bots than members."])
            .AddRequirement(g => g.MemberCount >= minimumMemberCount, this._localizer["Has less than {0} members.", minimumMemberCount]);

        var (guildRecord, hasGuild) = await guildRepository.HasGuildAsync(e.Guild.Id);

        if (hasGuild)
        {
            requirementsBuilder.AddRequirement(_ => guildRecord!.GuildState is not GuildStates.Restricted, "Guild is restricted!");
        }

        var requirements = requirementsBuilder.Build();

        await requirements.LeaveIfNotPassed(this._requirementsLocalizer);

        if (!requirements.PassedRequirements)
        {
            return;
        }

        string welcomeText;
        if (guildRecord?.GuildState is GuildStates.Verified)
        {
            welcomeText = this._localizer["WelcomeOldGuildTitle", TranslationFindingType.WithOnlyTextContext];
        }
        else
        {
            welcomeText = this._localizer["WelcomeGuildTitle", TranslationFindingType.WithOnlyTextContext];
            await guildRepository.AddGuildAsync(new Guild
            {
                GuildId = e.Guild.Id,
                GuildState = GuildStates.Verified,
            });

            await guildRepository.UnitOfWork.SaveChangesAsync();
        }
        
        var welcomeEmbedBuilder = new DiscordEmbedBuilder()
            .WithDescription(welcomeText)
            .WithColor(DiscordColor.Blurple);

        await guild.GetDefaultChannel()
            .SendMessageAsync(welcomeEmbedBuilder);
    }

    private static async Task<bool> HasMoreMembersThanBots(DiscordGuild guild)
    {
        var members = await guild.GetAllMembersAsync();
        var botQuery = from member in members.AsParallel()
            where member.IsBot
            select member;

        var botCount = botQuery.ToArray()
            .Length;

        var memberCount = members.Count - botCount;

        return memberCount > botCount;
    }
}