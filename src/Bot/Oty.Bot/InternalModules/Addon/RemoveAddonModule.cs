namespace Oty.Bot.InternalModules;

public partial class AddonManager
{
    [UsedImplicitly]
    public sealed class RemoveAddonModule : BaseVerifiedCommandModule<SlashInteractionContext>
    {
        public RemoveAddonModule(SlashInteractionContext context) : base(context)
        {
        }
    }
}