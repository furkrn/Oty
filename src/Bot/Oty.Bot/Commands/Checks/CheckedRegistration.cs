namespace Oty.Bot.Commands.Checks;

[PublicAPI]
public class CheckedRegistration : ICheckedRegistration
{
    [PublicAPI]
    public CheckedRegistration(bool canExecute)
    {
        this.CanExecute = canExecute;
    }

    [PublicAPI]
    public bool CanExecute { get; }
}