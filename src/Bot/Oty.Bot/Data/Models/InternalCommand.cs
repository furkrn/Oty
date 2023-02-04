namespace Oty.Bot.Data.Models;

[PrimaryKey(nameof(CommandType), nameof(Name))]
[PublicAPI]
public class InternalCommand
{
    [PublicAPI]
    public required ApplicationCommandType CommandType { get; set; }

    [PublicAPI]
    public required string Name { get; set; }

    [PublicAPI]
    public required ulong Id { get; set; }

    [PublicAPI]
    public required int HashCode { get; set; }

    [PublicAPI]
    public required ulong? GuildId { get; set; }
}