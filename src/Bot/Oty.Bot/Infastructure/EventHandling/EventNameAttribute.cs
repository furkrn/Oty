namespace Oty.Bot.Infastructure;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class EventNameAttribute : Attribute
{
    public EventNameAttribute(string eventName)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(eventName, nameof(eventName));

        this.Event = eventName;
    }

    public string Event { get; }
}