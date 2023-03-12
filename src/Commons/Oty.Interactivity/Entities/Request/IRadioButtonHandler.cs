namespace Oty.Interactivity.Entities;

public interface IRadioButtonHandler
{
    Task HandleButtonSelections(ComponentInteractionCreateEventArgs eventArgs);
}