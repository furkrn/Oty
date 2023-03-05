namespace Oty.Interactivity.Entities;

public class RadioButtonHandler
{
    protected int _current = 1;

    public RadioButtonHandler(IReadOnlyList<DiscordComponent> components, IDiscordMessageBuilder messageBuilder, int componentIndex)
    {
        ArgumentNullException.ThrowIfNull(components, nameof(components));
        ArgumentNullException.ThrowIfNull(messageBuilder, nameof(messageBuilder));

        if (componentIndex > 4)
        {
            throw new ArgumentOutOfRangeException(nameof(componentIndex), "Component index cannot be bigger than 4");
        }

        this.Components = components;
        this.MessageBuilder = messageBuilder;
        this.TargetComponentIndex = componentIndex;
    }

    public IDiscordMessageBuilder MessageBuilder { get; }

    public IReadOnlyList<DiscordComponent> Components { get; }

    public int TargetComponentIndex { get; }

    public virtual async Task HandleButtonSelections(ComponentInteractionCreateEventArgs eventArgs)
    {
        int maxValue = this.Components.Count - 1;
        if (Interlocked.CompareExchange(ref this._current, 0, maxValue) == 0)
        {
            this._current++;
        }

        var interactionResponseBuilder = new DiscordInteractionResponseBuilder(this.MessageBuilder);
        interactionResponseBuilder.ClearComponents();

        for (int i = 0; i < this.MessageBuilder.Components.Count; i++)
        {
            if (i == this.TargetComponentIndex)
            {
                var targetComponent = this.Components[this._current];

                interactionResponseBuilder.AddComponents(targetComponent);
            }
            else
            {
                interactionResponseBuilder.AddComponents(this.MessageBuilder.Components[i].Components);
            }
        }
        
        await eventArgs.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, interactionResponseBuilder);
    }
}