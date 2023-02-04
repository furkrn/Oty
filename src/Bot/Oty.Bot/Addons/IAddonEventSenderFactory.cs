namespace Oty.Bot.Addons;

public interface IAddonEventSenderFactory
{
    IAddonEventSender Create(DiscordClient client);
}