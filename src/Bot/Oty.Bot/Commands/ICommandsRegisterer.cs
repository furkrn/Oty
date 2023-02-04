namespace Oty.Bot.Commands;

[PublicAPI]
public interface ICommandsRegisterer
{
    [PublicAPI]
    Task RegisterCommandsAsync(IOtyCommandsExtension extension);
}