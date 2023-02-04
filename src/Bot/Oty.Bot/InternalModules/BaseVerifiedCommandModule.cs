namespace Oty.Bot.InternalModules;

[PublicAPI]
public abstract class BaseVerifiedCommandModule<TContext> : BaseCommandModule<TContext>
    where TContext : BaseInteractionCommandContext
{
    private readonly IStringLocalizer<SharedTranslation> _localizer;

    private readonly IServiceScopeFactory _scopeFactory;

    [UsedImplicitly]
    protected BaseVerifiedCommandModule(TContext context) : base(context)
    {
        this._scopeFactory= context.RegisteredServices!.GetRequiredService<IServiceScopeFactory>();
        this._localizer = context.RegisteredServices!.GetRequiredService<IStringLocalizer<SharedTranslation>>();
    }

    [PublicAPI]
    public DiscordInteraction? TosValidatedInteraction { get; private set; }

    public override async Task<bool> BeforeExecutionAsync()
    {
        await using var scopeFactory = this._scopeFactory.CreateAsyncScope();
        await using var repository = scopeFactory.ServiceProvider.GetRequiredService<IUserRepository>();

        var (user, isNotBanned) = await repository.TryLiftUserBan(this.Context.User.Id);

        CultureInfo.CurrentCulture = this.Context.GetUserCultureInfo();

        if (user is not null)
        {
            if (isNotBanned)
            {
                return true;
            }
            else
            {
                string banReason = user.BanReason ?? this._localizer["NoReason", TranslationFindingType.WithOnlyTextContext];

                var bannedEmbedBuilder = new DiscordEmbedBuilder()
                    .WithTitle(this._localizer["BanEmbedTitle", TranslationFindingType.WithOnlyTextContext])
                    .WithDescription(this._localizer["BanEmbedDescription", TranslationFindingType.WithOnlyTextContext, banReason, user.BanLiftTime!.Value])
                    .WithColor(DiscordColor.Red);

                var bannedInteractionResponseBuilder = new DiscordInteractionResponseBuilder()
                    .AsEphemeral()
                    .AddEmbed(bannedEmbedBuilder);

                await this.Context.CreateResponseAsync(bannedInteractionResponseBuilder);

                return false;
            }
        }

        string targetButtonId = "agree";
        var agreeComponent = new DiscordButtonComponent(ButtonStyle.Success, targetButtonId, this._localizer["Agree"], false, new("✅"));

        DiscordEmbed tosEmbedBuilder = new DiscordEmbedBuilder()
            .WithDescription(this._localizer["TosEmbedDescription", TranslationFindingType.WithOnlyTextContext])
            .WithColor(DiscordColor.Blurple);

        var firstTimeResponseBuilder = new DiscordInteractionResponseBuilder()
            .AddEmbed(tosEmbedBuilder)
            .AddComponents(agreeComponent)
            .AsEphemeral();

        await this.Context.CreateResponseAsync(firstTimeResponseBuilder);

        var originalMessage = await this.Context.GetOriginalResponseAsync();

        var clickRequest = new ClickInteractivityRequestBuilder()
            .SetTargetUser(this.Context.User)
            .WithTargetMessage(originalMessage)
            .WithTargetInteractionId(targetButtonId)
            .SetRepeative(false);

        var result = await this.Context.Client.GetInteractivityExtension()
            .HandleClickRequestAsync(clickRequest, TimeSpan.FromMinutes(1));

        if (result.IsTimedOut || !result.IsExecuted)
        {
            return false;
        }

        await repository.AddUserAsync(result.EventResult.User.Id, UserStates.TosAccepted);
        await repository.UnitOfWork.SaveChangesAsync();

        this.TosValidatedInteraction = result.EventResult.Interaction;

        return true;
    }
}