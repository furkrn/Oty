namespace Oty.CommandLib.Utilities;

/// <summary>
/// Represents utilites for expression & delegates.
/// </summary>
internal static class DelegateUtilities
{
    /// <summary>
    /// Converts method into a delegate.
    /// </summary>
    /// <param name="methodInfo">Info of the method to convert.</param>
    /// <param name="instance">Instance of the declaring type of the <paramref name="methodInfo"/>, required if it's not a static method</param>
    /// <returns>An delegate from <paramref name="methodInfo"/></returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="methodInfo"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="methodInfo"/> isn't static and <paramref name="instance"/> is null.</exception>
    internal static Delegate ToDelegate(this MethodInfo methodInfo, object? instance)
    {
        ArgumentNullException.ThrowIfNull(methodInfo, nameof(methodInfo));

        if (!methodInfo.IsStatic)
        {
            if (instance == null)
            {
                throw new ArgumentException("Instance of a non-static method cannot be null", nameof(instance));
            }

            if (methodInfo.DeclaringType != instance.GetType())
            {
                throw new ArgumentException($"Invalid type used for instance, instance should be a type of {methodInfo.DeclaringType}", nameof(instance));
            }
        }

        if (methodInfo.IsAbstract)
        {
            throw new ArgumentException("Method cannot be abstract.", nameof(methodInfo));
        }

        var methodParameters = methodInfo.GetParameters()
            .Select(p => p.ParameterType)
            .ToList();

        Type delegateType;
        if (methodInfo.ReturnType == typeof(void))
        {
            delegateType = Expression.GetActionType(methodParameters.ToArray());
        }
        else
        {
            methodParameters.Add(methodInfo.ReturnType);
            delegateType = Expression.GetFuncType(methodParameters.ToArray());

        }

        return instance != null
            ? Delegate.CreateDelegate(delegateType, instance, methodInfo)
            : Delegate.CreateDelegate(delegateType, methodInfo);
    }
}