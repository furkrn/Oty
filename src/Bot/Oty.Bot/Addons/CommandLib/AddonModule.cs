namespace Oty.Bot.Addons.CommandLib;

public sealed class AddonModule : BaseCommandModule<AddonContext>, IMetadataCreatable
{
    private readonly IAddonService? _addonService;

    private readonly IReceivedCheckCollection? _checkCollection;

    public AddonModule(AddonContext context) : base(context)
    {
        this._addonService = context.RegisteredServices?.GetRequiredService<IAddonService>();
        this._checkCollection = context.RegisteredServices?.GetRequiredService<IReceivedCheckCollection>();
    }

    public static BaseCommandMetadata CreateMetadata(IMetadataProvider metadataProvider)
    {
        throw new NotImplementedException(); // create metadata here from specified metadata provider.
    }

    public override Task<bool> BeforeExecutionAsync()
    {
        return this._checkCollection?.CheckAsync(this.Context!) ?? Task.FromResult(true);
    }

    public override Task ExecuteAsync(IReadOnlyDictionary<IMetadataOption, object?>? parameterCollection = null)
    {
        return base.ExecuteAsync(parameterCollection); // execute the specified one
    }

    public override Task AfterExecutionAsync()
    {
        return base.AfterExecutionAsync(); // remove from the barrier
    }
}