namespace Oty.Bot.Infastructure;

public class EventHandlerType
{
    public EventHandlerType(Type senderType, Type eventArgsType, IEnumerable<Type> eventHandlerTypes)
    {
        ArgumentNullException.ThrowIfNull(senderType, nameof(senderType));
        ArgumentNullException.ThrowIfNull(eventArgsType, nameof(eventArgsType));
        ArgumentNullException.ThrowIfNull(eventHandlerTypes, nameof(eventHandlerTypes));

        if (!typeof(AsyncEventArgs).IsAssignableFrom(eventArgsType))
        {
            throw new ArgumentException($"The specified type must be inherited from {nameof(AsyncEventArgs)}", nameof(eventArgsType));
        }

        if (!eventHandlerTypes.Any())
        {
            throw new ArgumentException($"Collection of handler types cannot be empty", nameof(eventHandlerTypes));
        }

        this.SenderType = senderType;
        this.EventArgsType = eventArgsType;
        this.EventHandlerTypes = eventHandlerTypes;
    }

    public Type SenderType { get; }

    public Type EventArgsType { get; }

    public IEnumerable<Type> EventHandlerTypes { get; }

    public void Deconstruct(out Type senderType, out Type eventArgsType, out IEnumerable<Type> eventHandlerTypes)
    {
        senderType = this.SenderType;
        eventArgsType = this.EventArgsType;
        eventHandlerTypes = this.EventHandlerTypes;
    }
}