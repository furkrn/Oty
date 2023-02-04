namespace Oty.Bot.Infastructure;

public class Limitation : ILimitation, IEquatable<Limitation>
{
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    protected readonly ConcurrentDictionary<DateTimeOffset, LimitationState> Limits = new();

    protected uint Uses;

    public Limitation(BaseCommandMetadata metadata, ILimitationLocation location, uint maximumUsage, TimeSpan limitationTime)
    {
        this.TargetCommand = metadata ?? throw new ArgumentNullException(nameof(metadata));
        this.Location = location ?? throw new ArgumentNullException(nameof(location));
        this.MaximumUsage = maximumUsage;
        this.LimitationTime = limitationTime;
        this.Expiration = DateTimeOffset.Now + limitationTime;
    }

    public BaseCommandMetadata TargetCommand { get; }

    public ILimitationLocation Location { get; }

    public uint MaximumUsage { get; }

    public TimeSpan LimitationTime { get; }

    public ILimitation? Next { get; private set; }

    protected DateTimeOffset Expiration { get; set; }

    public ILimitation SetNext(ILimitation? limitation)
    {
        this.Next = limitation;

        return limitation ?? this;
    }

    public virtual async Task IncreaseAsync(LimitationResultBuilder builder)
    {
        await this._semaphoreSlim.WaitAsync();

        this.TryReset(builder.ExecutedTime);

        bool isFailed = true;
        if (this.Uses < this.MaximumUsage)
        {
            uint uses = Interlocked.Increment(ref this.Uses);
            isFailed = false;

            this.Limits.TryAdd(builder.ExecutedTime, new(uses));
        }

        builder.WithState(isFailed, this.Location, this.Expiration);
        this._semaphoreSlim.Release();

        await (this.Next?.IncreaseAsync(builder) ?? Task.CompletedTask);
    }

    public virtual async Task RevertFromAsync(LimitationResult result)
    {
        await this._semaphoreSlim.WaitAsync();

        var now = DateTimeOffset.Now;
        if (!this.TryReset(now) && this.Limits.TryGetValue(result.ExecutionTime, out var limited))
        {
            Interlocked.Exchange(ref this.Uses, limited.Uses);
        }

        this._semaphoreSlim.Release();

        await (this.Next?.RevertFromAsync(result) ?? Task.CompletedTask);
    }

    public virtual bool Equals(Limitation? other)
       => other is not null &&
            this.TargetCommand == other.TargetCommand &&
            this.Location.Equals(other.Location) &&
            this.MaximumUsage == other.MaximumUsage &&
            this.LimitationTime == other.LimitationTime &&
            (this.Next is null || this.Next.Equals(other.Next));

    public override bool Equals(object? obj)
        => obj is Limitation other && this.Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(this.TargetCommand, this.Location, this.MaximumUsage, this.LimitationTime);

    protected virtual bool TryReset(DateTimeOffset time)
    {
        if (time >= this.Expiration)
        {
            Interlocked.Exchange(ref this.Uses, 0);
            this.Expiration = time + this.LimitationTime;
            this.Limits.Clear();

            return true;
        }

        return false;
    }

    public static bool operator ==(Limitation left, Limitation right)
        => left.Equals(right);

    public static bool operator !=(Limitation left, Limitation right)
        => !(left == right);

    protected record struct LimitationState(uint Uses);
}