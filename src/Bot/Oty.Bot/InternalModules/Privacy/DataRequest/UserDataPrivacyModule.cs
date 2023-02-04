namespace Oty.Bot.InternalModules;

public partial class PrivacyModule
{
    [UsedImplicitly]
    public sealed class UserDataPrivacyModule : BaseDataRequestPrivacyModule
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public UserDataPrivacyModule(SlashInteractionContext context) : base(context)
        {
            this._scopeFactory = context.RegisteredServices!.GetRequiredService<IServiceScopeFactory>();
        }

        public override Task ExecuteAsync(IReadOnlyDictionary<IMetadataOption, object?>? parameterCollection = null)
        {
            var scopeFactory = this._scopeFactory;
            ulong userId = this.Context.User.Id;

            return this.CreateDataResponseAsync(async () =>
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                await using var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                return await repository.GetUserAsync(userId); 
            });
        }
    }
}