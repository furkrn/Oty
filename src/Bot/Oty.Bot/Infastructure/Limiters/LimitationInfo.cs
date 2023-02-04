namespace Oty.Bot.Infastructure;

public readonly struct LimitationInfo
{
    public LimitationInfo(LimitationTypes type, TimeSpan limitationTime, uint maximumUses)
    {
        if (limitationTime == TimeSpan.Zero)
        {
            throw new ArgumentException("Value cannot be empty.", nameof(limitationTime));
        }

        if (maximumUses <= 0)
        {
            throw new ArgumentException("Value must be greater than 0.", nameof(maximumUses));
        }

        this.Type = type;
        this.LimitationTime = limitationTime;
        this.MaximumUses = maximumUses;
    }

    public uint MaximumUses { get; }

    public TimeSpan LimitationTime { get; }

    public LimitationTypes Type { get; }
}