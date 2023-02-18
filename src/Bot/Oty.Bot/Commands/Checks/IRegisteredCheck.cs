namespace Oty.Bot.Commands.Checks;

[PublicAPI]
public interface IRegisteredCheck
{
    Task<bool?> CheckAsync(DiscordClient client, IServiceProvider serviceProvider);
}

[PublicAPI]
public interface IRegisteredCheck<[UsedImplicitly] out TModule> : IRegisteredCheck
    where TModule : BaseCommandModule
{
}