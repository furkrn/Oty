namespace Oty.Things.Trie;

public sealed class Trie<T> : ITrie<T>, IReadOnlyTrie<T>
{
    private readonly Dictionary<TrieKey, Trie<T>> _children = new();

    private readonly List<T> _avaliableValues = new();

    public IReadOnlyList<T> AvaliableValues => this._avaliableValues;

    public IEnumerable<T> this[string query]
    {
        get
        {
            return this.Get(query);
        }
    }
    
    public void Add(string query, T value)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(query, nameof(query));
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        var currentTrie = this;

        for (int i = 0; i < query.Length; i++)
        {
            var key = new TrieKey(query[i], i, i >= query.Length - 1);

            if (!currentTrie._children.TryGetValue(key, out var newTrie))
            {
                newTrie = new Trie<T>();

                currentTrie._children.Add(key, newTrie);
            }

            if (key.IsLast)
            {
                currentTrie._avaliableValues.Add(value);
            }

            currentTrie = newTrie;
        }
    }

    public IEnumerable<T> Get(string query)
    {
        var child = this;

        for (int i = 0; i < query.Length; i++)
        {
            var trieKey = new TrieKey(query[i], i, i >= query.Length);

            if (!child._children.TryGetValue(trieKey, out child!))
            {
                return Enumerable.Empty<T>();
            }
        }

        return EnumerableExtensions.Traverse(child, c => c._children.Values)
            .SelectMany(c => c._avaliableValues);
    }

    public readonly struct TrieKey : IEquatable<TrieKey>
    {
        internal TrieKey(char key, int position, bool isLast)
        {
            this.Key = key;
            this.Position = position;
            this.IsLast = isLast;
        }

        public char Key { get; }

        public bool IsLast { get; }

        public int Position { get; }

        public void Deconstruct(out char key, out bool isLast, out int position)
        {
            key = this.Key;
            isLast = this.IsLast;
            position = this.Position;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is TrieKey other && this.Equals(other);
        }

        public bool Equals(TrieKey other)
        {
            return this.IsLast == other.IsLast &&
                this.Key == other.Key &&
                this.Position == other.Position;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.IsLast, this.Key, this.Position);
        }

        public static bool operator ==(TrieKey left, TrieKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TrieKey left, TrieKey right)
        {
            return !(left == right);
        }
    }
}