namespace Oty.Bot.Infastructure;
public static class BaseCommandContextExtensions
{
    [PublicAPI]
    public static CultureInfo GetUserCultureInfo(this BaseInteractionCommandContext context)
    {
        ArgumentNullException.ThrowIfNull(nameof(context));

        return context.Interaction.Locale!.GetCultureInfo(context.RegisteredServices!.GetRequiredService<IPoProvider>());
    }

    [PublicAPI]
    public static CultureInfo GetGuildCultureInfo(this BaseInteractionCommandContext context)
    {
        ArgumentNullException.ThrowIfNull(nameof(context));

        return context.Interaction.GuildLocale!.GetCultureInfo(context.RegisteredServices!.GetRequiredService<IPoProvider>());
    }

    [PublicAPI]
    public static CultureInfo GetGuildCultureInfo(this DiscordGuild guild, IPoProvider poProvider)
    {
        ArgumentNullException.ThrowIfNull(guild, nameof(guild));
        ArgumentNullException.ThrowIfNull(poProvider, nameof(poProvider));

        return guild.PreferredLocale.GetCultureInfo(poProvider);
    }

    private static CultureInfo GetCultureInfo(this string cultureName, IPoProvider poProvider)
    {
        var culture = CultureInfo.GetCultureInfo(cultureName);

        var (_, location) = poProvider.GetCatalog(culture);

        return location.IsSuccessfull
            ? culture
            : CultureInfo.GetCultureInfo("en-US", true);
    }
}