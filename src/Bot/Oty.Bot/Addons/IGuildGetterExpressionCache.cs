namespace Oty.Bot.Addons;

public interface IGuildGetterExpressionCache
{
    Func<TArgs, DiscordGuild> GetGuildFunc<TArgs>()
        where TArgs : AsyncEventArgs;

    void Clear();
}