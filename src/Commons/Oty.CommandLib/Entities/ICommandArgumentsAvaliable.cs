namespace Oty.CommandLib.Entities;

/// <summary>
/// Defines the whether command has options that might be required to be converted using <see cref="IParameterTypeParser"/>.
/// </summary>
public interface ICommandArgumentsAvaliable
{
}

/// <inheritdoc/>
/// <typeparam name="TOption">The type of the option</typeparam>
public interface ICommandArgumentsAvaliable<out TOption> : ICommandArgumentsAvaliable
    where TOption : ICommandOption
{
    /// <summary>
    /// Gets the options of the command.
    /// </summary>
    IReadOnlyList<TOption>? Options { get; }
}