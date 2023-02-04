namespace Oty.Bot.Infastructure;

public readonly struct LimitationResult : IEquatable<LimitationResult>
{
    private readonly ILimitation? _source;

    public LimitationResult(ILimitation source, LimitationResultBuilder builder)
    {
        this._source = source;
        this.ExecutionTime = builder.ExecutedTime;
        this.Locations = builder.Locations;
        this.Exceedded = this.Locations.Any(kp => kp.Value);
        this.Expiration = builder.Expiration;
    }

    public DateTimeOffset ExecutionTime { get; }

    public IReadOnlyDictionary<ILimitationLocation, bool> Locations { get; }

    public bool Exceedded { get; }

    public DateTimeOffset Expiration { get; }

    public Task RevertChangesAsync()
    {
        return (this._source?.RevertFromAsync(this) ?? Task.CompletedTask);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is LimitationResult other && this.Equals(other);

    public bool Equals(LimitationResult result)
        => (this._source?.Equals(result._source)).GetValueOrDefault();

    public override int GetHashCode()
    {
        return HashCode.Combine(this._source);
    }

    public static bool operator ==(LimitationResult left, LimitationResult right)
        => left.Equals(right);
    
    public static bool operator !=(LimitationResult left, LimitationResult right)
        => !(left == right);
}