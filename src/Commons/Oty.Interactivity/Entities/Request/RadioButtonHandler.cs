namespace Oty.Interactivity.Entities;

public class RadioButtonHandler : IRadioButtonHandler
{
    protected int _current;

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

    public virtual Task HandleButtonSelections(ComponentInteractionCreateEventArgs eventArgs)
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
            var messageComponents = this.MessageBuilder.Components[i].Components;

            if (i == this.TargetComponentIndex)
            {
                var listedComponents = messageComponents.ToList();
                var targetComponent = this.Components[this._current];
                var indexedComponent = listedComponents.Select((c, i) => new { Component = c, Index = i})
                    .Where(a => a.Component.CustomId == targetComponent.CustomId)
                    .FirstOrDefault();

                listedComponents[indexedComponent.Index] = targetComponent;

                interactionResponseBuilder.AddComponents(listedComponents);
            }
            else
            {
                interactionResponseBuilder.AddComponents(messageComponents);
            }
        }

        return this.CreateResponseAsync(eventArgs, interactionResponseBuilder);
    }

    protected virtual Task CreateResponseAsync(ComponentInteractionCreateEventArgs eventArgs, DiscordInteractionResponseBuilder interactionResponseBuilder)
    {
        return eventArgs.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, interactionResponseBuilder);
    }
}