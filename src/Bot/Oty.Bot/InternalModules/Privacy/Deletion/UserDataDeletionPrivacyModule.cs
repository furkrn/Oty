namespace Oty.Bot.InternalModules;

public partial class PrivacyModule
{
    public partial class DataDeletionPrivacyModule
    {
        [UsedImplicitly]
        public sealed class UserDataDeletionPrivacyModule : BaseDataDeletionPrivacyModule
        {
            private readonly IStringLocalizer<UserDataDeletionPrivacyModule> _localizer;

            public UserDataDeletionPrivacyModule(SlashInteractionContext context) : base(context)
            {
                this._localizer = context.RegisteredServices!.GetRequiredService<IStringLocalizer<UserDataDeletionPrivacyModule>>();
            }

            public override async Task ExecuteAsync(IReadOnlyDictionary<IMetadataOption, object?>? parameterCollection = null)
            {
                CultureInfo.CurrentCulture = this.Context.GetUserCultureInfo();

                await this.ValidatedInteraction!.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);

                await using var scope = this.ScopeFactory.CreateAsyncScope();
                await using var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                var (user, isNotBanned) = await repository.TryLiftUserBan(this.Context.User.Id);

                if (user is null)
                {
                    await CreateMessage(new DiscordEmbedBuilder()
                        .WithDescription(this._localizer["NoUserDataDescription", TranslationFindingType.WithOnlyTextContext])
                        .WithColor(DiscordColor.Blurple));

                    return;
                }

                if (!isNotBanned)
                {
                    await CreateMessage(new DiscordEmbedBuilder()
                        .WithDescription(this._localizer["BannedUserDataDescription", TranslationFindingType.WithOnlyTextContext, user.BanLiftTime!])
                        .WithColor(DiscordColor.Red));

                    return;
                }

                await repository.RemoveUserAsync(user);
                await repository.UnitOfWork.SaveChangesAsync();

                await CreateMessage(new DiscordEmbedBuilder()
                    .WithDescription(this._localizer["UserDataRemovedDescription", TranslationFindingType.WithOnlyTextContext])
                    .WithColor(DiscordColor.Blurple));

                Task CreateMessage(DiscordEmbed embed)
                    => this.ValidatedInteraction!.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                        .AddEmbed(embed));

            }
        }
    }
}