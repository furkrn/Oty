namespace Oty.Interactivity.Entities;

public class ComponentPaginatorSubBuilder : IComponentCommandBuilder
{
    private readonly IEnumerable<DiscordSelectComponentOption> _options;

    private readonly List<DiscordComponent[]>? _afterComponents = new();

    private Func<ComponentInteractionCreateEventArgs, Task>? _selectBoxInvoker;

    private Paginator _paginator = new Paginator();

    private string _selectBoxCustomId = "pagination_select";

    private PaginationButtons _buttons = new()
    {
        FirstPageButton = new(ButtonStyle.Primary, "first", "|<<"),
        BackPageButton = new(ButtonStyle.Primary, "back", "<<"),
        NextPageButton = new(ButtonStyle.Primary, "next", ">>"),
        LastPageButton = new(ButtonStyle.Primary, "last", ">>|"),
    };

    public ComponentPaginatorSubBuilder(IEnumerable<DiscordSelectComponentOption> options)
    {
        this._options = options;
    }

    public static ComponentPaginatorSubBuilder CreateGenericBuilder<T>(IEnumerable<T> items, Func<T, DiscordSelectComponentOption> selector)
    {
        ArgumentNullException.ThrowIfNull(items, nameof(items));
        ArgumentNullException.ThrowIfNull(selector, nameof(selector));

        return new ComponentPaginatorSubBuilder(items.Select(selector));
    }

    public ComponentPaginatorSubBuilder AddComponentsAfterSelectBox(params DiscordComponent[] componentsArray)
    {
        ArgumentNullException.ThrowIfNull(componentsArray, nameof(componentsArray));

        this._afterComponents.Add(componentsArray);

        return this;
    }
    
    public ComponentPaginatorSubBuilder AddComponentsAfterSelectBox(IEnumerable<DiscordComponent> components)
    {
        ArgumentNullException.ThrowIfNull(components, nameof(components));

        return this.AddComponentsAfterSelectBox(componentsArray: components.ToArray());
    }

    public ComponentPaginatorSubBuilder OnSelectBoxSelection(Func<ComponentInteractionCreateEventArgs, Task> selectBoxInvoker)
    {
        this._selectBoxInvoker = selectBoxInvoker;

        return this;
    }

    public ComponentPaginatorSubBuilder WithPaginator(Paginator paginator)
    {
        this._paginator = paginator ?? throw new ArgumentNullException(nameof(paginator));

        return this;
    }

    public ComponentPaginatorSubBuilder WithPaginationButtons(PaginationButtons buttons)
    {
        this._buttons = buttons ?? throw new ArgumentNullException(nameof(buttons));

        return this;
    }

    public ComponentPaginatorSubBuilder WithSelectBoxCustomId(string customId)
    {
        if (string.IsNullOrWhiteSpace(customId))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(customId));
        }

        this._selectBoxCustomId = customId;

        return this;
    }

    public IEnumerable<KeyValuePair<ComponentInteractivityRequest.ComponentKey, ComponentInteractivityRequest.ComponentInteractivityInvoker?>> Build(MessageComponents? components)
    {
        int messageRowComponentCount = 2 + this._afterComponents.Count;
        if (components.MessageBuilder.Components.Count + messageRowComponentCount > 5)
        {
            throw new InvalidOperationException("");
        }

        var selectorItems = this._options.Chunk(25)
            .ToArray();

        var pages = new List<InteractionPage>();

        for (uint i = 0; i < selectorItems.Length; i++)
        {
            var selectBox = new DiscordSelectComponent(this._selectBoxCustomId, null, selectorItems[i]);

            var builder = new DiscordInteractionResponseBuilder(components.MessageBuilder)
                .AddComponents(selectBox);

            if (selectorItems.Length > 1)
            {
                builder.AddComponents(this._buttons.GetButtonsAsCollectionWithCondition(i, (uint)selectorItems.Length));
            }
            else if (this._buttons.MiddleButton is not null)
            {
                builder.AddComponents(this._buttons.MiddleButton);
            }

            var page = new InteractionPage(builder);

            if (i is 0)
            {
                components.TryAddComponents(new[] { selectBox }, out _);
                components.TryAddComponents(this._buttons.GetButtonsAsCollection().ToArray(), out _);
            }

            pages.Add(page);
        }

        var commands = new List<KeyValuePair<ComponentInteractivityRequest.ComponentKey, ComponentInteractivityRequest.ComponentInteractivityInvoker?>>();

        this._paginator.Pages = pages;

        var selectBoxKey = new ComponentInteractivityRequest.ComponentKey(this._selectBoxCustomId, ComponentInvokationType.SelectBox);

        var selectBoxInvoker = new ComponentInteractivityRequest.ComponentInteractivityInvoker(this._selectBoxCustomId, ComponentInvokationType.SelectBox, this._selectBoxInvoker);

        commands.Add(new(selectBoxKey, selectBoxInvoker));

        if (selectorItems.Length > 1)
        {
            commands.AddRange(this._buttons.GetAsCommands(this._paginator));
        }

        return commands;
    }
}