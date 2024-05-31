using System.Diagnostics.CodeAnalysis;

namespace System.Collections.Generic;

public interface IReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
{
	IEnumerable<TKey> Keys { get; }

	IEnumerable<TValue> Values { get; }

	TValue this[TKey key] { get; }

	bool ContainsKey(TKey key);

	bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);
}
