namespace Oty.Bot.Utilities;

public static class TrieHelper
{
    public static IReadOnlyTrie<T> CreateTrieUsing<T>(IEnumerable<T> source, Func<T, string> querySelector)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(querySelector, nameof(querySelector));

        var trie = new Trie<T>();

        foreach (var item in source)
        {
            trie.Add(querySelector(item), item);
        }

        return trie;
    }
}