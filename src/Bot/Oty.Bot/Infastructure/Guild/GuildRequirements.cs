namespace Oty.Bot.Infastructure;

public sealed class GuildRequirements
{
    public GuildRequirements(GuildRequirementsBuilder builder)
    {
        this.Guild = builder.Guild;
        this.Requirements = builder.Requirements;
        this.PassedRequirements = builder.Requirements.All(c => c.Value);
    }

    public DiscordGuild Guild { get; }

    public bool PassedRequirements { get; }

    public IReadOnlyDictionary<string, bool> Requirements { get; }

    public async Task LeaveIfNotPassed(IStringLocalizer<GuildRequirements> localizer)
    {
        ArgumentNullException.ThrowIfNull(localizer, nameof(localizer));

        if (this.PassedRequirements)
        {
            return;
        }

        var channel = await this.Guild.CreateTextChannelAsync(localizer["Oty_Left_Guild"]);

        var stringBuilder = new StringBuilder();

        foreach (var requirement in this.Requirements.Where(c => !c.Value).ToArray())
        {
            stringBuilder.Append("-> ")
                .Append(requirement.Key)
                .Append('\n');
        }
        
        DiscordEmbed embed = new DiscordEmbedBuilder()
            .WithTitle(localizer["⚠️ Your guild didn't meet following requirement :", Plural.From("⚠️ Your guild didn't meet following requirements :", this.Requirements.Count)])
            .WithDescription(stringBuilder.ToString());

        var messageBuilder = new DiscordMessageBuilder()
            .WithContent(localizer["GuildRequirementsFailContent", TranslationFindingType.WithOnlyTextContext])
            .AddEmbed(embed);

        await channel.SendMessageAsync(messageBuilder);

        await Guild.LeaveAsync();
    }

}