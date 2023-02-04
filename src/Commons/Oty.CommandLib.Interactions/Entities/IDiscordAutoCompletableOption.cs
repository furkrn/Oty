namespace Oty.CommandLib.Interactions.Entities;

/// <summary>
/// Represents the command option requires an <typeparamref name="TCommand"/> as an autocomplete command.
/// </summary>
/// <typeparam name="TCommand">Type of the autocomplete command.</typeparam>
public interface IDiscordAutoCompleteableOption : IDiscordMetadataOption
{
    bool IDiscordMetadataOption.IsAutoComplete => true;

    /// <summary>
    /// Gets the <see cref="TCommand"/> autocomplete command of this option.
    /// </summary>

    public string? AutoCompleteCommand { get; }
}