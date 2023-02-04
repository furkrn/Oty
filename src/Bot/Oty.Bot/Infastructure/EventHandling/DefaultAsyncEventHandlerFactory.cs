namespace Oty.Bot.Infastructure;

public class DefaultAsyncEventHandlerFactory : IAsyncEventHandlerFactory
{
    private readonly IServiceProvider _serviceProvider;

    private readonly ConcurrentDictionary<Type, Func<IServiceProvider, IAsyncEventHandler>> _instanceFuncCache = new();

    private static readonly MethodInfo GenericGetMethodInfo = typeof(ServiceProviderServiceExtensions).GetMethods()
        .Where(c => c.Name == nameof(ServiceProviderServiceExtensions.GetRequiredService) && c.IsGenericMethodDefinition)
        .First();

    public DefaultAsyncEventHandlerFactory(IServiceProvider serviceProvider)
    {
        this._serviceProvider = serviceProvider;
    }

    public IAsyncEventHandler Create(Type type)
    {
        if (!this._instanceFuncCache.TryGetValue(type, out var func))
        {
            if (this._serviceProvider.GetService(type) is not null)
            {
                func = (serviceProvider) => (IAsyncEventHandler)serviceProvider.GetRequiredService(type);
            }
            else
            {
                var parameterExpression = Expression.Parameter(typeof(IServiceProvider), "serviceProvider"); 

                var ctor = type.GetConstructors()[0];
                var ctorParameterTypes = ctor.GetParameters();

                var allExpressions = new List<Expression>();

                foreach (var ctorParameterInfo in ctorParameterTypes)
                {
                    // TODO : Check for 'IServiceProvider' type.
                    var variableExpression = Expression.Variable(ctorParameterInfo.ParameterType, ctorParameterInfo.Name);
                    var callMethod = GenericGetMethodInfo.MakeGenericMethod(ctorParameterInfo.ParameterType);
                    var assignExpression = Expression.Assign(variableExpression, Expression.Call(null, callMethod, parameterExpression));

                    allExpressions.AddRange(new Expression[] { variableExpression, assignExpression });
                }

                var variables = allExpressions.OfType<ParameterExpression>();
                var expressions = allExpressions.Union(variables);
                allExpressions.Add(Expression.New(ctor, variables));

                func = Expression.Lambda<Func<IServiceProvider, IAsyncEventHandler>>(Expression.Block(variables, expressions), parameterExpression).Compile();
            }

            this._instanceFuncCache.TryAdd(type, func);
        }

        return func(this._serviceProvider);
    }
}