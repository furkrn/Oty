namespace Oty.Bot.InternalModules;

public partial class AddonManager
{
    [UsedImplicitly]
    public sealed class ManageAddonModule : BaseVerifiedCommandModule<SlashInteractionContext>
    {
        public ManageAddonModule(SlashInteractionContext context) : base(context)
        {
        }
    }
}