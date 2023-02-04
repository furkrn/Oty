namespace Oty.Bot.Infastructure;

public class AsyncEventHandlerOptions
{
    public required IReadOnlyList<EventHandlerType> RegisteredOptions { get; set; }
}