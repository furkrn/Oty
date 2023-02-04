namespace Oty.Bot.Infastructure;

public sealed class LimitedCommandMetadataProvider : MetadataProvider
{
    private readonly ConcurrentDictionary<Type, HashSet<LimitationInfo>> _specifiedLimits = new();

    public LimitedCommandMetadataProvider(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }

    public static readonly Func<IServiceProvider, IMetadataProvider> DefaultFactory = 
        serviceProvider => new LimitedCommandMetadataProvider(serviceProvider);

    public IReadOnlyDictionary<Type, HashSet<LimitationInfo>> SpecifiedLimits => this._specifiedLimits;

    public LimitedCommandMetadataProvider AddLimit<TModule>(LimitationInfo info)
        where TModule : BaseCommandModule
    {
        this._specifiedLimits.AddOrUpdate(typeof(TModule), 
            _ => new() { info },
            (_, hs) =>
        {
            hs.Add(info);
            return hs;
        } );

        return this;
    }

    public LimitedCommandMetadataProvider AddLimit<TModule>(LimitationTypes type, TimeSpan timeSpan, uint maxUses)
        where TModule : BaseCommandModule
    {
        return this.AddLimit<TModule>(new(type, timeSpan, maxUses));
    }

    public LimitedCommandMetadataProvider AddLimits<TModule>(params LimitationInfo[] limits)
        where TModule : BaseCommandModule
    {
        this._specifiedLimits.GetOrAdd(typeof(TModule), 
            _ => new())
            .UnionWith(limits);

        return this;
    }

    public LimitedCommandMetadataProvider AddLimits<TModule>(IEnumerable<LimitationInfo> limitEnumerable)
        where TModule : BaseCommandModule
    {
        ArgumentNullException.ThrowIfNull(limitEnumerable, nameof(limitEnumerable));

        this.AddLimits<TModule>(limitEnumerable.ToArray());

        return this;
    }
}