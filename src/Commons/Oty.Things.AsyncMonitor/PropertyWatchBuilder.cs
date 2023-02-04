namespace Oty.Things.AsyncMonitor;

/// <summary>
/// A builder class for creating <see cref="PropertyWatch"/> instances.
/// /// </summary>
/// <typeparam name="TOptions">The type of the option.</typeparam>
/// <typeparam name="TProperty">The type of the property to monitor.</typeparam>
[PublicAPI]
public class PropertyWatchBuilder<TOptions, TProperty>
    where TOptions : class
{
    private PropertyInfo? _propertyInfo;

    /// <summary>
    /// Sets the target property of the instance using the lambda expression to get its property.
    /// </summary>
    /// <param name="expression">The expression for getting the target property.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the specified parameter is not a <see cref="MemberExpression"/></exception>
    /// <exception cref="ArgumentException">Thrown when the specified expression's member is not a property.</exception>
    /// <exception cref="ArgumentException">Thrown when the specified property's getter or setter is not available or public.</exception>
    /// <returns></returns>
    [PublicAPI]
    public PropertyWatchBuilder<TOptions, TProperty> FromPropertyFunc(Expression<Func<TOptions, TProperty>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression, nameof(expression));

        if (expression is not MemberExpression memberExpression)
        {
            throw new ArgumentException("Expression must be an member expression.", nameof(expression));
        }

        if (memberExpression.Member is not PropertyInfo property)
        {
            throw new ArgumentException("Expression must target an property.");
        }

        if (property.GetMethod?.IsPublic is not true ||
            property.SetMethod?.IsPublic is not true)
        {
            throw new ArgumentException("Target property must contain public getter and setter.");
        }

        this._propertyInfo = property;

        return this;
    }

    /// <summary>
    /// Sets the target property of the instance from its name.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when there's no property with the specified name.</exception>
    /// <returns></returns>
    [PublicAPI]
    public PropertyWatchBuilder<TOptions, TProperty> FromPropertyName(string propertyName)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(propertyName, nameof(propertyName));

        var property = typeof(TOptions).GetProperty(propertyName) 
            ?? throw new ArgumentException($"Specified public property is not specified on {nameof(TOptions)}");

        this._propertyInfo = property;

        return this;
    }

    /// <summary>
    /// Sets the target property of the instance from available <typeparamref name="TProperty"/> type.
    /// <exception cref="InvalidOperationException">Thrown when the target options contains more than 1 property with the <typeparamref name="TProperty"/> type.</exception>
    /// </summary>
    /// <returns></returns>
    [PublicAPI]
    public PropertyWatchBuilder<TOptions, TProperty> FromPropertyType()
    {
        var properties = typeof(TOptions)
            .GetProperties()
            .Where(c => c.PropertyType == typeof(TProperty))
            .ToArray();

        if (properties.Length is not 1)
        {
            throw new InvalidOperationException($"Specified {nameof(TOptions)} has more properties with type of {nameof(TProperty)}.");
        }

        this._propertyInfo = properties[0];

        return this;
    }

    /// <summary>
    /// Creates an instance of <see cref="PropertyWatch"/> using the specified values.
    /// </summary>
    /// <returns>An instance of <see cref="PropertyWatch"/></returns>
    [PublicAPI]
    public PropertyWatch Build()
    {
        if (this._propertyInfo is null)
        {
            throw new ArgumentException("Specified target property must be specified!");
        }

        return new PropertyWatch(typeof(TOptions), this._propertyInfo);
    }

    public static implicit operator PropertyWatch(PropertyWatchBuilder<TOptions, TProperty> builder)
        => builder.Build();
}