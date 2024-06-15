using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Theraot.Collections.Specialized;

namespace System.Collections.Generic;

[Serializable]
[ComVisible(false)]
[DebuggerNonUserCode]
[DebuggerDisplay("Count={Count}")]
public class DictionaryEx<TKey, TValue> : Dictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IHasComparer<TKey>
{
	public DictionaryEx()
	{
	}

	public DictionaryEx(IDictionary<TKey, TValue> dictionary)
		: base(dictionary)
	{
	}

	public DictionaryEx(KeyValuePair<TKey, TValue>[] dictionary)
	{
		if (dictionary == null)
		{
			throw new ArgumentNullException("dictionary");
		}
		for (int i = 0; i < dictionary.Length; i++)
		{
			KeyValuePair<TKey, TValue> keyValuePair = dictionary[i];
			Add(keyValuePair.Key, keyValuePair.Value);
		}
	}

	public DictionaryEx(KeyValuePair<TKey, TValue>[] dictionary, IEqualityComparer<TKey> comparer)
		: base(comparer)
	{
		if (dictionary == null)
		{
			throw new ArgumentNullException("dictionary");
		}
		for (int i = 0; i < dictionary.Length; i++)
		{
			KeyValuePair<TKey, TValue> keyValuePair = dictionary[i];
			Add(keyValuePair.Key, keyValuePair.Value);
		}
	}

	public DictionaryEx(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
		: base(dictionary, comparer)
	{
	}

	public DictionaryEx(IEqualityComparer<TKey> comparer)
		: base(comparer)
	{
	}

	public DictionaryEx(int capacity)
		: base(capacity)
	{
	}

	public DictionaryEx(int capacity, IEqualityComparer<TKey> comparer)
		: base(capacity, comparer)
	{
	}

	protected DictionaryEx(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
	
	IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => base.Keys;

	IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => base.Values;

	// IEqualityComparer<TKey> IHasComparer<TKey>.Comparer()
	// {
	// 	return base.Comparer;
	// }
}