namespace Oty.Bot.Infastructure;

[PublicAPI]
public sealed class SnowflakeLimitationLocation : IEquatable<SnowflakeLimitationLocation>, ILimitationLocation
{
    public SnowflakeLimitationLocation(SnowflakeObject snowflake, LimitationTypes type)
    {
        this.Snowflake = snowflake ?? throw new ArgumentNullException(nameof(snowflake));
        this.Type = type;
    }

    [PublicAPI]
    public SnowflakeObject Snowflake { get; }

    [PublicAPI]
    public LimitationTypes Type { get; }

    public override bool Equals(object? obj)
    {
        return obj is SnowflakeLimitationLocation other && this.Equals(other);
    }

    public bool Equals(SnowflakeLimitationLocation? other)
    {
        return other is not null &&
            this.Snowflake.Equals(other.Snowflake) &&
            this.Type == other.Type;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Snowflake, this.Type);
    }

    public static bool operator ==(SnowflakeLimitationLocation left, SnowflakeLimitationLocation right)
        => left.Equals(right);

    public static bool operator !=(SnowflakeLimitationLocation left, SnowflakeLimitationLocation right)
        => !(left == right);
}