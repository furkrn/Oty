namespace Oty.Bot.InternalModules;

public partial class AddonManager
{
    [UsedImplicitly]
    public sealed class AddAddonModule : BaseVerifiedCommandModule<SlashInteractionContext>
    {
        public AddAddonModule(SlashInteractionContext context) : base(context)
        {
        }
    }
}