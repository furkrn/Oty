namespace Oty.Bot.Commands;

[PublicAPI]
public sealed class CommandsConfiguration
{
    public required IReadOnlyDictionary<Type, ModuleMetadataHelper> RegisteredTypes { get; set; }
}