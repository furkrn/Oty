namespace Oty.Bot.InternalModules;

[UsedImplicitly]
public sealed class HelpModule : BaseVerifiedCommandModule<SlashInteractionContext>, IMetadataCreatable
{
    private readonly IStringLocalizer<HelpModule> _localizer;

    public HelpModule(SlashInteractionContext context) : base(context)
    {
        this._localizer = context.RegisteredServices!.GetRequiredService<IStringLocalizer<HelpModule>>();
    }
    
    public static BaseCommandMetadata CreateMetadata(IMetadataProvider metadataProvider)
    {
        if (metadataProvider is LimitedCommandMetadataProvider limiterProvider)
        {
            limiterProvider.AddLimit<HelpModule>(LimitationTypes.UserWise, TimeSpan.FromSeconds(30), 1);
        }

        return new InteractionCommandBuilder<HelpModule>(ApplicationCommandType.SlashCommand)
            .WithName("help")
            .WithDescription("Get help! NOW!")
            .WithPermissions(Permissions.UseApplicationCommands)
            .LocalizeFrom(metadataProvider);
    }

    public override async Task ExecuteAsync(IReadOnlyDictionary<IMetadataOption, object?>? parameterCollection = null)
    {
        if (this.TosValidatedInteraction is not null)
        {
            await this.Context.DeleteOriginalResponseAsync();
        }

        CultureInfo.CurrentCulture = this.Context.GetUserCultureInfo();

        var embed = new DiscordEmbedBuilder()
            .WithColor(DiscordColor.Blurple)
            .WithDescription(this._localizer["HelpModuleEmbedDescription", TranslationFindingType.WithOnlyTextContext]);

        var responseBuilder = new DiscordInteractionResponseBuilder()
            .AddEmbed(embed);

        if (this.Context.Member is null)
        {
            var dmOnlyEmbed = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Blurple)
                .WithDescription(this._localizer["HelpModuleDMDescription", TranslationFindingType.WithOnlyTextContext]);

            responseBuilder.AddEmbed(dmOnlyEmbed);
        }

        await (this.TosValidatedInteraction ?? this.Context.Interaction).CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, responseBuilder);
    }

}