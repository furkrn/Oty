namespace Oty.Bot.Commands;

[PublicAPI]
public sealed class CommandsBuilder
{
    private readonly Dictionary<Type, Func<IServiceProvider, IMetadataProvider>> _commands = new();

    public IReadOnlyDictionary<Type, Func<IServiceProvider, IMetadataProvider>> Commands => this._commands;

    public CommandsBuilder AddModule<TModule>(Func<IServiceProvider, IMetadataProvider>? func = null)
        where TModule : BaseCommandModule, IMetadataCreatable
    {
        func ??= services => new MetadataProvider(services); 

        this._commands.Add(typeof(TModule), func);

        return this;
    }
}