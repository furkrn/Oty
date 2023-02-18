namespace Oty.Bot;

[PublicAPI]
public class BotConfiguration
{
    #nullable disable

    [PublicAPI]
    [Required(AllowEmptyStrings = false)]
    public string SecretToken { get; set; }

    [PublicAPI]
    [Required]
    [Range(1, int.MaxValue)]
    public int ShardCount { get; set; }

    [PublicAPI]
    [Required(AllowEmptyStrings = false)]
    [Range(1, ulong.MaxValue)]
    public ulong SpecialGuildId { get; set; }

    [PublicAPI]
    [Required(AllowEmptyStrings = false)]
    [Range(1, ulong.MaxValue)]
    public ulong ReportChannel { get; set; }

    [PublicAPI]
    [Required(AllowEmptyStrings = false)]
    public ulong BotSupportServerId { get; set; }

    [PublicAPI]
    [Required(AllowEmptyStrings = false)]
    public string Activity { get; set; }

    [PublicAPI]
    public ActivityType ActivityStatus { get; set; }

    [PublicAPI]
    public UserStatus UserStatus { get; set; }

    [PublicAPI]
    [Required(AllowEmptyStrings = false)]
    [Range(1, ushort.MaxValue)]
    public ushort MinimumMemberRequirement { get; set; }

    [PublicAPI]
    [Required(AllowEmptyStrings = false)]
    public string SupportInvite { get; set; }

    [PublicAPI]
    [Required(AllowEmptyStrings = false)]
    public string ConnectionString { get; set; }
}