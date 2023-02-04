namespace Oty.Bot.Data.Models;

public sealed class Guild
{
    [Key]
    public required ulong GuildId { get; set; }

    public required GuildStates GuildState { get; set; }

    public string? RestrictionReason { get; set; }
}