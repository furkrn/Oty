namespace Oty.Bot.InternalModules;

public partial class AddonManager
{
    [UsedImplicitly]
    public sealed class ListAddonModule : BaseVerifiedCommandModule<SlashInteractionContext>
    {
        public ListAddonModule(SlashInteractionContext context) : base(context)
        {
        }
    }
}