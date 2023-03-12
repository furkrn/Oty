namespace Oty.Interactivity.Entities;

public class MessageComponents : IEquatable<MessageComponents>
{
    private readonly List<DiscordComponent[]> _components = new();

    public MessageComponents(IDiscordMessageBuilder messageBuilder)
    {
        this.MessageBuilder = messageBuilder;
    }

    public IReadOnlyList<DiscordComponent[]> Components { get; }

    public IDiscordMessageBuilder MessageBuilder { get; }

    public bool TryAddComponents(DiscordComponent[] components, [MaybeNullWhen(false)] out int? index)
    {
        index = null;

        if (components.Length > 5)
        {
            return false;
        }

        int componentCount = this.MessageBuilder.Components.Count + 1;
        bool canAdd = componentCount <= 5;

        if (canAdd)
        {
            this.MessageBuilder.AddComponents(components);
            this._components.Add(components);

            index = this.MessageBuilder.Components.Count - 1;
        }

        return canAdd;
    }

    public override bool Equals(object? obj)
    {
        return obj is MessageComponents other && this.Equals(other);
    }

    public virtual bool Equals(MessageComponents? other)
    {
        return other is not null &&
            EqualityComparer<IDiscordMessageBuilder>.Default.Equals(this.MessageBuilder, other.MessageBuilder);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.MessageBuilder);
    }

    public static bool operator ==(MessageComponents left, MessageComponents right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(MessageComponents left, MessageComponents right)
    {
        return !(left == right);
    }
}