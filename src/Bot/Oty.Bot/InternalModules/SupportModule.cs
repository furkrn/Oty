namespace Oty.Bot.InternalModules;

[UsedImplicitly]
public sealed class SupportModule : BaseCommandModule<SlashInteractionContext>, IMetadataCreatable
{
    private readonly BotConfiguration _configuration;

    private readonly IStringLocalizer<SupportModule> _localizer;

    public SupportModule(SlashInteractionContext context) : base(context)
    {
        this._configuration = context.RegisteredServices!.GetRequiredService<IOptionsSnapshot<BotConfiguration>>().Value;
        this._localizer = context.RegisteredServices!.GetRequiredService<IStringLocalizer<SupportModule>>();
    }

    public static BaseCommandMetadata CreateMetadata(IMetadataProvider metadataProvider)
    {
        return new InteractionCommandBuilder<SupportModule>(ApplicationCommandType.SlashCommand)
            .WithName("support")
            .WithDescription("You want help with us? Sure.")
            .WithPermissions(Permissions.UseApplicationCommands)
            .AllowPrivateChannels()
            .LocalizeFrom(metadataProvider);
    }

    public override async Task ExecuteAsync(IReadOnlyDictionary<IMetadataOption, object?>? parameterCollection = null)
    {
        CultureInfo.CurrentCulture = this.Context.GetUserCultureInfo();

        var embed = new DiscordEmbedBuilder()
            .WithColor(DiscordColor.Orange)
            .WithDescription(this._localizer["SupportEmbedDescription", TranslationFindingType.WithOnlyTextContext, this._configuration.SupportInvite]);

        var interactionResponseBuilder = new DiscordInteractionResponseBuilder()
            .AsEphemeral()
            .AddEmbed(embed);

        await this.Context.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, interactionResponseBuilder);
    }
}