namespace Oty.Bot.Commands;

[PublicAPI]
public sealed class CommandsBuilder
{
    private readonly Dictionary<Type, ModuleMetadataHelper> _commands = new();

    public IReadOnlyDictionary<Type, ModuleMetadataHelper> Commands => this._commands;

    public CommandsBuilder AddModule<TModule>(Func<IServiceProvider, IMetadataProvider>? func = null, IRegisteredCheck<TModule>? check = null)
        where TModule : BaseCommandModule, IMetadataCreatable
    {
        func ??= (serviceProvider) => new MetadataProvider(serviceProvider);
        this._commands.Add(typeof(TModule), new(func, check));

        return this;
    }

    public CommandsBuilder AddCheck<TModule>(IRegisteredCheck<TModule> check)
        where TModule : BaseCommandModule, IMetadataCreatable
    {
        ArgumentNullException.ThrowIfNull(check, nameof(check));

        this._commands.Add(typeof(TModule), new(null, check));

        return this;
    }
}