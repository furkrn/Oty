namespace Oty.Interactivity.Entities;

public sealed class ComponentInteractivityBuilder
{
    private readonly Dictionary<ComponentInteractivityRequest.ComponentKey, ComponentInteractivityRequest.ComponentInteractivityInvoker?> _targets = new();

    private readonly MessageComponents? _components;

    private DiscordMessage _message;
    
    public ComponentInteractivityBuilder(MessageComponents? components = null)
    {
        this._components = components;
    }

    public bool IsRepeative { get; set; }

    public DiscordUser? TargetUser { get; set; }

    public DiscordMessage Message
    {
        get
        {
            return this._message;
        }
        set
        {
            ArgumentNullException.ThrowIfNull(value, nameof(this.Message));

            this._message = value;
        }
    }

    public IReadOnlyDictionary<ComponentInteractivityRequest.ComponentKey, ComponentInteractivityRequest.ComponentInteractivityInvoker?> TargetComponents => this._targets;

    public ComponentInteractivityBuilder AsRepeative(bool repeative = true)
    {
        this.IsRepeative = repeative;

        return this;
    }

    public ComponentInteractivityBuilder WithTargetUser(DiscordUser user)
    {
        this.TargetUser = user;

        return this;
    }

    public ComponentInteractivityBuilder WithTargetMessage(DiscordMessage message)
    {
        this.Message = message;

        return this;
    }

    public ComponentInteractivityBuilder FromSubBuilder(Func<IComponentCommandBuilder> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return this.AddTargetComponents(builder().Build(this._components)); 
    }

    public ComponentInteractivityBuilder FromSubBuilder(IComponentCommandBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        return this.AddTargetComponents(builder.Build(this._components));
    }

    public ComponentInteractivityBuilder AddTargetComponent(DiscordComponent component, Func<ComponentInteractionCreateEventArgs, Task>? invoker)
    {
        if (component.Type is (ComponentType.ActionRow or ComponentType.FormInput))
        {
            throw new ArgumentException("Specified component cannot be a modal or an action row.", nameof(component));
        }

        var invokationType = component.Type;

        var componentKey = new ComponentInteractivityRequest.ComponentKey(component.CustomId, invokationType);

        var componentInvoker = new ComponentInteractivityRequest.ComponentInteractivityInvoker(component.CustomId, invokationType, invoker);

        return this.AddTargetComponent(componentKey, componentInvoker);
    }

    public ComponentInteractivityBuilder AddTargetComponent(string componentId, ComponentType type, ComponentInteractivityRequest.ComponentInteractivityInvoker? invoker)
    {
        return this.AddTargetComponent(new()
        {
            TargetComponentId = componentId,
            ComponentInvokationType = type,
        }, invoker);
    }

    public ComponentInteractivityBuilder AddTargetComponent(ComponentInteractivityRequest.ComponentKey componentKey, ComponentInteractivityRequest.ComponentInteractivityInvoker? invoker)
    {
        ArgumentNullException.ThrowIfNull(invoker, nameof(invoker));

        this._targets.Add(componentKey, invoker);

        return this;
    }

    public ComponentInteractivityBuilder AddTargetComponents(IEnumerable<KeyValuePair<ComponentInteractivityRequest.ComponentKey, ComponentInteractivityRequest.ComponentInteractivityInvoker?>> commands)
    {
        ArgumentNullException.ThrowIfNull(commands, nameof(commands));

        foreach (var (key, invoker) in commands)
        {
            this._targets.Add(key, invoker);
        }

        return this;
    }
    
    public ComponentInteractivityRequest Build()
    {
        if (this._message == null)
        {
            throw new InvalidOperationException("Cannot build the request without its target message.");
        }

        return new()
        {
            TargetComponents = this._targets,
            TargetUser = this.TargetUser,
            TargetMessage = this._message,
            IsRepeative = this.IsRepeative,
        };
    }

    public static implicit operator ComponentInteractivityRequest(ComponentInteractivityBuilder builder)
        => builder!.Build();
}