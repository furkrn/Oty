namespace Oty.Things;

public static class EnumerableExtensions
{
    /// <summary>
    /// Traverses a tree. startes from root of it and using a defined function to get the children.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sourceEnumerableFunc"></param>
    /// <typeparam name="T">Type of the source.</typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="sourceEnumerableFunc"/> is <see langword="null"/></exception>
    public static IEnumerable<T> Traverse<T>(T? source, Func<T, IEnumerable<T>?> sourceEnumerableFunc)
    {
        ArgumentNullException.ThrowIfNull(sourceEnumerableFunc, nameof(sourceEnumerableFunc));

        if (source is null)
        {
            yield break;
        }

        var queue = new Queue<T>();

        queue.Enqueue(source);

        while (queue.Count > 0)
        {
            var dequeue = queue.Dequeue();
            yield return dequeue;

            var items = sourceEnumerableFunc(dequeue);

            if (items != null)
            {
                foreach (var item in items)
                {
                    queue.Enqueue(item);
                }
            }
        }
    }

    /// <summary>
    /// Traverses a tree in a reverse, startes from root and using a user defined function to get children.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sourceEnumerableFunc"></param>
    /// <typeparam name="T">Type of the source.</typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="sourceEnumerableFunc"/> is <see langword="null"/></exception>
    public static IEnumerable<T> ReverseTraverse<T>(T? source, Func<T, IEnumerable<T>?> sourceEnumerableFunc)
    {
        ArgumentNullException.ThrowIfNull(sourceEnumerableFunc, nameof(sourceEnumerableFunc));

        if (source is null)
        {
            yield break;
        }

        var stack = new Stack<T>();

        stack.Push(source);

        while (stack.Count > 0)
        {
            var poppedResult = stack.Pop();
            yield return poppedResult;

            var items = sourceEnumerableFunc(poppedResult);

            if (items != null)
            {
                foreach (var item in items)
                {
                    stack.Push(item);
                }
            }
        }
    }

    /// <summary>
    /// Gets all nested items on the item.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sourceFunc"></param>
    /// <typeparam name="T">Type of the source.</typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is <see langword="null"/></exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="sourceFunc"/> is <see langword="null"/></exception>
    public static IEnumerable<T> GetAllNestedItems<T>(T source, Func<T, T> sourceFunc)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(sourceFunc, nameof(sourceFunc));

        var item = source;

        while (item != null)
        {
            yield return item;
            item = sourceFunc(item);
        }
    }

    public static async Task<IEnumerable<T>> SkipWhileAsync<T>(IEnumerable<T> source, Func<T, Task<bool>> predicate)
    {
        var list = new List<T>();

        foreach (var item in source)
        {
            if (await predicate(item).ConfigureAwait(false))
            {
                continue;
            }

            list.Add(item);
        }

        return list;
    }
}