namespace Oty.Bot.Commands;

[PublicAPI]
public sealed class CommandsConfiguration
{
    public required IReadOnlyDictionary<Type, Func<IServiceProvider, IMetadataProvider>> RegisteredTypes { get; set; }
}