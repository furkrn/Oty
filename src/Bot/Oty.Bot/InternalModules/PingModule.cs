namespace Oty.Bot.InternalModules;

[UsedImplicitly]
public sealed class PingModule : BaseCommandModule<SlashInteractionContext>, IMetadataCreatable
{
    private readonly IStringLocalizer<PingModule> _localizer;

    public PingModule(SlashInteractionContext context) : base(context)
    {
        this._localizer = context.RegisteredServices!.GetRequiredService<IStringLocalizer<PingModule>>();
    }

    public static BaseCommandMetadata CreateMetadata(IMetadataProvider metadataProvider)
    {
        return new InteractionCommandBuilder<PingModule>(ApplicationCommandType.SlashCommand)
            .WithName("ping")
            .WithDescription("Wanna play some ping pong?")
            .WithPermissions(Permissions.UseApplicationCommands)
            .AllowPrivateChannels()
            .LocalizeFrom(metadataProvider);
    }

    // resharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

    public override async Task ExecuteAsync(IReadOnlyDictionary<IMetadataOption, object?>? parameterCollection = null)
    {
        CultureInfo.CurrentCulture = this.Context.GetUserCultureInfo();

        var interactionResponseBuilder = new DiscordInteractionResponseBuilder()
            .WithContent(this._localizer["PingInteractionContent", TranslationFindingType.WithOnlyTextContext, this.Context.Client.Ping]);

        if (this.Context.Guild is null)
        {
            var embedBuilder = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Orange)
                .WithDescription(this._localizer["PingInteractionGuildEmbedDescription", TranslationFindingType.WithOnlyTextContext]);

            interactionResponseBuilder.AddEmbed(embedBuilder);
        }

        await this.Context.CreateResponseAsync(interactionResponseBuilder);
    }
}