namespace Oty.Bot.Infastructure;

public interface ILimitation
{
    ILimitationLocation Location { get; }

    ILimitation? Next { get; }

    ILimitation SetNext(ILimitation? limitation);

    Task IncreaseAsync(LimitationResultBuilder builder);

    Task RevertFromAsync(LimitationResult result);
}