namespace Oty.Bot;

[PublicAPI]
public class OtyClientMonitor : BaseAsyncOptionsPropertyMonitor<BotConfiguration, int, OtyClientMonitor>, IWatchedProperty<BotConfiguration, int>
{
    private readonly IClientCollection _stack;

    public OtyClientMonitor(IClientCollection stack, IAsyncOptionsPropertyHandlerFactory factory) : base(factory)
    {
        this._stack = stack;
    }

    public static PropertyWatch Build(PropertyWatchBuilder<BotConfiguration, int> builder)
    {
        return builder.FromPropertyType();
    }

    public override async Task ExecuteAsync(ComparedValue<BotConfiguration, int> values)
    {
        if (values.NewValue > this._stack.Count)
        {
            await CreateArrayAndTaskWhenAny(values.NewValue - this._stack.Count, () => this._stack.PushAndConnectAsync());
        }
        else if (this._stack.Count > values.NewValue)
        {
            await CreateArrayAndTaskWhenAny(this._stack.Count - values.NewValue, () => this._stack.DisconnectAndPopAsync());
        }
    }

    private static async Task CreateArrayAndTaskWhenAny(int count, Func<Task> funcTask)
    {
        var tasks = new Task[count];

        for (int i = 0; i < count; i++)
        {
            tasks[i] = funcTask();
        }

        await (await Task.WhenAny(tasks));
    }
}