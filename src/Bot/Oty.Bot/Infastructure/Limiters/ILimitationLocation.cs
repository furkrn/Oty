namespace Oty.Bot.Infastructure;

[PublicAPI]
public interface ILimitationLocation
{
    [PublicAPI]
    LimitationTypes Type { get; }
}