namespace Oty.Bot.InternalModules;

public partial class PrivacyModule
{
    [UsedImplicitly]
    public sealed partial class DataDeletionPrivacyModule : BaseCommandModule<SlashInteractionContext>
    {
        public DataDeletionPrivacyModule(SlashInteractionContext context) : base(context)
        {
        }
    }
}