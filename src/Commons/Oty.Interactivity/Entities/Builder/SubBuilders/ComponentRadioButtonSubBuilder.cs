namespace Oty.Interactivity.Entities;

[PublicAPI]
public sealed class ComponentRadioButtonSubBuilder : IComponentCommandBuilder
{
    private readonly List<DiscordComponent> _components = new();
    
    private Func<IReadOnlyList<DiscordComponent>, IDiscordMessageBuilder, int, RadioButtonHandler> _factory = (l, m, i) => new(l, m, i);

    [PublicAPI]
    public ComponentRadioButtonSubBuilder()
    {
    }

    [PublicAPI]
    public ComponentRadioButtonSubBuilder(DiscordComponent firstComponent)
    {
        ArgumentNullException.ThrowIfNull(firstComponent, nameof(firstComponent));

        this._components.Add(firstComponent);
    }

    [PublicAPI]
    public IReadOnlyList<DiscordComponent> Components => this._components;

    [PublicAPI]
    public ComponentRadioButtonSubBuilder AddSelectedComponent(DiscordComponent component)
    {
        ArgumentNullException.ThrowIfNull(component, nameof(component));

        if (component.Type is (ComponentType.ActionRow or ComponentType.FormInput))
        {
            throw new ArgumentException("Cannot add component type that is modal or an action row.", nameof(component));
        }

        if (this._components.Count > 0 && component.CustomId != this._components[0].CustomId)
        {
            throw new ArgumentException("Component's custom id must be the same as the other components in this builder.", nameof(component));
        }

        this._components.Add(component);

        return this;
    }

    [PublicAPI]
    public ComponentRadioButtonSubBuilder AddSelectedComponents(params DiscordComponent[] componentArray)
    {
        ArgumentNullException.ThrowIfNull(componentArray, nameof(componentArray));

        if (componentArray.Length is 0)
        {
            throw new ArgumentException("Value cannot be empty.", nameof(componentArray));
        }

        if (componentArray.Any(c => c.Type is (ComponentType.FormInput or ComponentType.ActionRow)))
        {
            throw new ArgumentException("Component array cannot contain component with modal or action row.", nameof(componentArray));
        }

        string targetId = this._components.Count > 0
            ? this._components[0].CustomId
            : componentArray[0].CustomId;

        if (!componentArray.All(c => c.CustomId == targetId))
        {
            throw new ArgumentException("All of the component's custom ids must be the same in this builder.", nameof(componentArray));
        }

        return this;
    }

    [PublicAPI]
    public ComponentRadioButtonSubBuilder AddSelectedComponents(IEnumerable<DiscordComponent> components)
    {
        ArgumentNullException.ThrowIfNull(components, nameof(components));

        return this.AddSelectedComponents(componentArray: components.ToArray());
    }

    [PublicAPI]
    public ComponentRadioButtonSubBuilder WithHandlerFactory(Func<IReadOnlyList<DiscordComponent>, IDiscordMessageBuilder, int, RadioButtonHandler> factory)
    {
        ArgumentNullException.ThrowIfNull(factory, nameof(factory));
        this._factory = factory;

        return this;
    }

    [PublicAPI]
    public IEnumerable<KeyValuePair<ComponentInteractivityRequest.ComponentKey, ComponentInteractivityRequest.ComponentInteractivityInvoker?>> Build(MessageComponents? components)
    {
        if (components is null)
        {
            throw new InvalidOperationException("Message components are required for building radio buttons.");
        }

        if (this._components.Count is 0)
        {
            throw new InvalidOperationException("Cannot build an radio button component without anything on it.");
        }

        var firstComponent = this._components[0];

        if (components.MessageBuilder.Components.SelectMany(c => c.Components).Any(c => c.CustomId == firstComponent.CustomId))
        {
            throw new InvalidOperationException("Provided message builder contains components with the same id.");
        }

        if (!components.TryAddComponents(new[] { firstComponent }, out int? index))
        {
            throw new InvalidOperationException("Provided message builder cannot contain more components.");
        }

        var radioButtonHandler = this._factory(this._components, components.MessageBuilder, index.Value);

        return this._components.Select(c => KeyValuePair.Create(new ComponentInteractivityRequest.ComponentKey(c.CustomId, GetInvokationType(c.Type)),
            new ComponentInteractivityRequest.ComponentInteractivityInvoker(c.CustomId, GetInvokationType(c.Type), radioButtonHandler.HandleButtonSelections)))
            .DistinctBy(c => c.Key);

        static ComponentInvokationType GetInvokationType(ComponentType type)
        {
            return type is ComponentType.Button
                ? ComponentInvokationType.Button
                : ComponentInvokationType.SelectBox;
        }
    }
}