namespace Oty.CommandLib.Interactions.Entities;

public sealed class AutoCompleteInteractionCommand : BaseCommandMetadata
{
    internal AutoCompleteInteractionCommand(InteractionCommandBuilder builder, Type moduleType) : base(builder.CommandName, moduleType) 
    {
    }
}