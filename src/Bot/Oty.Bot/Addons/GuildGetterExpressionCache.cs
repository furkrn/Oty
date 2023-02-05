namespace Oty.Bot.Addons;

public class GuildGetterExpressionCache : IGuildGetterExpressionCache
{
    private readonly ConcurrentDictionary<Type, Func<AsyncEventArgs, DiscordGuild>> _expressionCache = new();

    public Func<TArgs, DiscordGuild> GetGuildFunc<TArgs>()
        where TArgs : AsyncEventArgs
    {
        var type = typeof(TArgs);

        return this._expressionCache.GetOrAdd(type, CreateExpression);
    }

    public void Clear()
    {
        this._expressionCache.Clear();
    }

    private static Func<AsyncEventArgs, DiscordGuild> CreateExpression(Type type)
    {
        var property = type.GetProperties()
            .FirstOrDefault(c => c.PropertyType == typeof(DiscordGuild));

        if (property is null)
        {
            throw new InvalidOperationException();
        }

        var expressionParameter = Expression.Parameter(typeof(AsyncEventArgs), "eventArgs");
        var memberExpression = Expression.Property(expressionParameter, property);

        return Expression.Lambda<Func<AsyncEventArgs, DiscordGuild>>(expressionParameter, expressionParameter)
            .Compile();
    }
}