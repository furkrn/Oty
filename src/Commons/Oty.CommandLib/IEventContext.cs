namespace Oty.CommandLib;

/// <summary>
/// Defines whether related commands needs type parsing with event.
/// </summary>
/// <typeparam name="TEvent">Type of the event that is received.</typeparam>
public interface IEventContext<out TEvent>
    where TEvent : DiscordEventArgs
{
    /// <summary>
    /// Gets the event received.
    /// </summary>
    TEvent Event { get; }
}