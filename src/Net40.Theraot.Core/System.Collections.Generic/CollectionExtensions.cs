using System.Diagnostics.CodeAnalysis;

namespace System.Collections.Generic;

public static class CollectionExtensions
{
	[return: MaybeNull]
	public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key) where TKey : notnull
	{
		return dictionary.GetValueOrDefault(key, default(TValue));
	}

	[return: MaybeNull]
	public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, [AllowNull] TValue defaultValue) where TKey : notnull
	{
		if (dictionary == null)
		{
			throw new ArgumentNullException("dictionary");
		}
		TValue value;
		return dictionary.TryGetValue(key, out value) ? value : defaultValue;
	}

	public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value) where TKey : notnull
	{
		if (dictionary == null)
		{
			throw new ArgumentNullException("dictionary");
		}
		if (!dictionary.ContainsKey(key))
		{
			dictionary.Add(key, value);
			return true;
		}
		return false;
	}

	public static bool Remove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, [MaybeNullWhen(false)] out TValue value) where TKey : notnull
	{
		if (dictionary == null)
		{
			throw new ArgumentNullException("dictionary");
		}
		if (dictionary.TryGetValue(key, out value))
		{
			dictionary.Remove(key);
			return true;
		}
		value = default(TValue);
		return false;
	}
}
