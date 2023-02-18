namespace Oty.Bot.InternalModules.Checks;

public class ReportModuleCheck : IRegisteredCheck<ReportModule>
{
    private BotConfiguration? _configuration;

    public async Task<bool?> CheckAsync(DiscordClient client, IServiceProvider serviceProvider)
    {
        this._configuration ??= serviceProvider.GetRequiredService<IOptions<BotConfiguration>>()
            .Value;

        var specialGuild = client.Guilds.FirstOrDefault(c => c.Key == this._configuration.SpecialGuildId);

        if (specialGuild.Value is null)
        {
            return null;
        }   

        var channels = await specialGuild.Value.GetChannelsAsync();
        var reportChannel = channels.FirstOrDefault(c => c.Id == this._configuration.ReportChannel);

        if (reportChannel is null)
        {
            return false;
        }

        return true;
    }
}