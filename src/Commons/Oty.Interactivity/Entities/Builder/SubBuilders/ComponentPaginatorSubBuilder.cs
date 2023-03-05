namespace Oty.Interactivity.Entities;

public sealed class ComponentPaginatorSubBuilder<TSource> : IComponentCommandBuilder
{
    private readonly IEnumerable<TSource> _items;

    private Func<TSource, SelectBoxItem> _selector;

    private Func<ComponentInteractionCreateEventArgs, Task>? _selectBoxInvoker;

    private Paginator _paginator = new Paginator();

    private DiscordInteractionResponseBuilder _responseBuilder = new DiscordInteractionResponseBuilder()
        .WithContent("Paginate using buttons below:");

    private string _selectBoxCustomId = "pagination_select";

    private PaginationButtons _buttons = new()
    {
        FirstPageButton = new(ButtonStyle.Primary, "first", "|<<"),
        BackPageButton = new(ButtonStyle.Primary, "back", "<<"),
        NextPageButton = new(ButtonStyle.Primary, "next", ">>"),
        LastPageButton = new(ButtonStyle.Primary, "last", ">>|"),
    };

    public ComponentPaginatorSubBuilder(IEnumerable<TSource> items)
    {
        this._items = items ?? throw new ArgumentNullException(nameof(items));
    }

    // TODO : Use properties as well...

    public ComponentPaginatorSubBuilder<TSource> WithSelectBoxItem(Func<TSource, SelectBoxItem> selectBoxItem)
    {
        this._selector = selectBoxItem ?? throw new ArgumentNullException(nameof(selectBoxItem));

        return this;
    }

    public ComponentPaginatorSubBuilder<TSource> OnSelectBoxSelection(Func<ComponentInteractionCreateEventArgs, Task> selectBoxInvoker)
    {
        this._selectBoxInvoker = selectBoxInvoker;

        return this;
    }

    public ComponentPaginatorSubBuilder<TSource> WithPaginator(Paginator paginator)
    {
        this._paginator = paginator ?? throw new ArgumentNullException(nameof(paginator));

        return this;
    }

    public ComponentPaginatorSubBuilder<TSource> WithBaseMessageBuilder(DiscordInteractionResponseBuilder responseBuilder)
    {
        this._responseBuilder = responseBuilder ?? throw new ArgumentNullException(nameof(responseBuilder));

        return this;
    }

    public ComponentPaginatorSubBuilder<TSource> WithPaginationButtons(PaginationButtons buttons)
    {
        this._buttons = buttons ?? throw new ArgumentNullException(nameof(buttons));

        return this;
    }

    public ComponentPaginatorSubBuilder<TSource> WithSelectBoxCustomId(string customId)
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
        if (this._selector == null)
        {
            throw new InvalidOperationException("Selector cannot be null!");
        }

        var selectorItems = this._items.Select(this._selector)
            .Chunk(25)
            .ToArray();

        var pages = new List<InteractionPage>();

        for (uint i = 0; i < selectorItems.Length; i++)
        {
            var selectBoxItems = selectorItems[i].Select(s => new DiscordSelectComponentOption(s.SelectBoxItemLabel, s.SelectBoxItemId));

            var selectBox = new DiscordSelectComponent(this._selectBoxCustomId, null, selectBoxItems);

            var builder = Clone(this._responseBuilder, out var rowComponents)
                .AddComponents(selectBox);

            foreach (var rowComponent in rowComponents)
            {
                builder.AddComponents(rowComponent.Components);
            }

            if (selectorItems.Length > 1)
            {
                builder.AddComponents(this._buttons.GetButtonsAsCollectionWithCondition(i, (uint)selectorItems.Length));
            }

            var page = new InteractionPage(builder);

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

    private static DiscordInteractionResponseBuilder Clone(DiscordInteractionResponseBuilder responseBuilder, out IEnumerable<DiscordActionRowComponent> components)
    {
        var newResponseBuilder = new DiscordInteractionResponseBuilder()
            .AddAutoCompleteChoices(responseBuilder.Choices)
            .AddEmbeds(responseBuilder.Embeds)
            .AddFiles(responseBuilder.Files.ToDictionary(f => f.FileName, f => f.Stream))
            .AddMentions(responseBuilder.Mentions)
            .AsEphemeral(responseBuilder.IsEphemeral)
            .WithContent(responseBuilder.Content)
            .WithTTS(responseBuilder.IsTTS);

        if (!string.IsNullOrWhiteSpace(responseBuilder.Title))
        {
            newResponseBuilder.WithTitle(responseBuilder.Title);
        }

        if (!string.IsNullOrWhiteSpace(responseBuilder.CustomId))
        {
            newResponseBuilder.WithCustomId(responseBuilder.CustomId);
        }

        components = responseBuilder.Components;
        return newResponseBuilder;
    }
}