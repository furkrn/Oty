namespace Oty.CommandLib.Handling;

/// <summary>
/// Represents a parser executor.
/// </summary>
public interface ICommandParser : IDisposable
{
    /// <summary>
    /// Gets the registered parsers
    /// </summary>
    [PublicAPI]
    IReadOnlyList<IParameterTypeParser> RegisteredParsers { get; }

    /// <summary>
    /// Configures the parser with the extension.
    /// </summary>
    /// <param name="extension"></param>
    /// <exception cref="InvalidOperationException">Thrown when parser is already configured.</exception>
    [PublicAPI]
    void Configure(IOtyCommandsExtension extension);
    
    /// <summary>
    /// Adds a parameter parser to the command parser.
    /// </summary>
    /// <param name="parser">Parser to add.</param>
    [PublicAPI]
    void AddParser(IParameterTypeParser parser);

    /// <summary>
    /// Removes a parameter parser from the command parser.
    /// </summary>
    /// <param name="parser">Parser to remove.</param>
    [PublicAPI]
    void RemoveParser(IParameterTypeParser parser);

    /// <summary>
    /// Parses and executes a method where parsing received options is required.
    /// </summary>
    /// <param name="moduleInstance">Instance of the command module</param>
    /// <param name="metadataOptionValues">Received options from executor.</param>
    /// <typeparam name="TContext">Context of the command module</typeparam>
    [PublicAPI]
    Task ParseAndExecuteCommandAsync<TContext>(BaseCommandModule<TContext> moduleInstance, IEnumerable<KeyValuePair<IMetadataOption, object?>>? metadataOptionValues)
        where TContext : BaseCommandContext;
}