namespace Oty.Bot.Infastructure;

public sealed class LimitationResultBuilder
{
    private readonly ILimitation _source;

    private readonly Dictionary<ILimitationLocation, bool> _locations = new();

    private readonly List<DateTimeOffset> _expirations = new();

    public LimitationResultBuilder(ILimitation source, DateTimeOffset executedTime)
    {
        this._source = source ?? throw new ArgumentNullException(nameof(source));
        this.ExecutedTime = executedTime;
    }

    public DateTimeOffset ExecutedTime { get; }

    public IReadOnlyDictionary<ILimitationLocation, bool> Locations => this._locations;

    public DateTimeOffset Expiration
        => this._expirations.Max();

    public LimitationResultBuilder WithState(bool isFailed, ILimitationLocation location, DateTimeOffset expiration)
    {
        ArgumentNullException.ThrowIfNull(location, nameof(location));

        this._locations.TryAdd(location, isFailed);
        this._expirations.Add(expiration);

        return this;
    }

    public LimitationResult Build()
    {
        return new(this._source, this);
    }

    public static implicit operator LimitationResult(LimitationResultBuilder builder)
        => builder.Build();
}