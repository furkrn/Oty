namespace Oty.Bot.Data.Models;

[PublicAPI]
public class User
{
    [Key]
    [PublicAPI]
    public required ulong Id { get; set; }

    [PublicAPI]
    public required UserStates UserState { get; set; }

    [PublicAPI]
    public string? BanReason { get; set; }

    [PublicAPI]
    public DateTime? BanLiftTime { get; set; }
}