namespace Oty.Bot;

[PublicAPI]
public readonly struct DiscordStatus : IEquatable<DiscordStatus>
{
    [PublicAPI]
    public DiscordStatus(string activity, ActivityType activityType, UserStatus userStatus)
    {
        this.Activity = activity;
        this.ActivityType = activityType;
        this.UserStatus = userStatus;
    }

    [PublicAPI]
    public string Activity { get; }

    [PublicAPI]
    public ActivityType ActivityType { get; }

    [PublicAPI]
    public UserStatus UserStatus { get; }

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is DiscordStatus other && this.Equals(other);

    public bool Equals(DiscordStatus other)
        => this.Activity == other.Activity &&
            this.ActivityType == other.ActivityType &&
            this.UserStatus == other.UserStatus;

    public override int GetHashCode()
        => HashCode.Combine(this.Activity, this.ActivityType, this.UserStatus);

    public static bool operator ==(DiscordStatus left, DiscordStatus right)
        => left.Equals(right);
        
    public static bool operator !=(DiscordStatus left, DiscordStatus right)
        => !(left == right);
}