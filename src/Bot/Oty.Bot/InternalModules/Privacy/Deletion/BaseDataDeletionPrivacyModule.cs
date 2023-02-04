namespace Oty.Bot.InternalModules;

[PublicAPI]
public abstract class BaseDataDeletionPrivacyModule : BaseCommandModule<SlashInteractionContext>
{
    private readonly IStringLocalizer<BaseDataDeletionPrivacyModule> _localizer;

    protected BaseDataDeletionPrivacyModule(SlashInteractionContext context) : base(context)
    {
        this._localizer = context.RegisteredServices!.GetRequiredService<IStringLocalizer<BaseDataDeletionPrivacyModule>>();
        this.ScopeFactory = context.RegisteredServices!.GetRequiredService<IServiceScopeFactory>();
    }

    [PublicAPI]
    public DiscordInteraction? ValidatedInteraction { get; private set; }

    [PublicAPI]
    protected IServiceScopeFactory ScopeFactory { get; }

    public override async Task<bool> BeforeExecutionAsync()
    {
        CultureInfo.CurrentCulture = this.Context.GetUserCultureInfo();

        DiscordEmbed embed = new DiscordEmbedBuilder()
            .WithTitle(this._localizer["Confirm deletion?"])
            .WithDescription(this._localizer["ConfirmationDescription", TranslationFindingType.WithOnlyTextContext])
            .WithColor(DiscordColor.Red);

        string acceptButtonId = "accept";
        var acceptButton = new DiscordButtonComponent(ButtonStyle.Danger, acceptButtonId, this._localizer["Accept and delete the related data"], false, new("✅"));

        var interactionResponseBuilder = new DiscordInteractionResponseBuilder()
            .AddComponents(acceptButton)
            .AddEmbed(embed)
            .AsEphemeral();

        await this.Context.CreateResponseAsync(interactionResponseBuilder);

        var message = await this.Context.GetOriginalResponseAsync();

        var interactivityBuilder = new ClickInteractivityRequestBuilder()
            .SetTargetUser(this.Context.User)
            .WithTargetMessage(message)
            .WithTargetInteractionId(acceptButtonId)
            .SetRepeative(false);

        var result = await this.Context.Client.GetInteractivityExtension()
            .HandleClickRequestAsync(interactivityBuilder, TimeSpan.FromMinutes(1));

        bool success = result.IsExecuted && !result.IsTimedOut;

        if (success)
        {
            this.ValidatedInteraction = result.EventResult.Interaction;
        }
        else
        {
            await this.Context.DeleteOriginalResponseAsync();
        }

        return success;
    }
}