using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Theraot.Core;

namespace Theraot.Collections.ThreadSafe;

public sealed class CacheDict<TKey, TValue> where TKey : class
{
	private sealed class Entry
	{
		internal readonly int Hash;

		internal readonly TKey Key;

		internal readonly TValue Value;

		internal Entry(int hash, TKey key, TValue value)
		{
			Hash = hash;
			Key = key;
			Value = value;
		}
	}

	private readonly Entry[] _entries;

	private readonly Func<TKey, TValue>? _valueFactory;

	public TValue this[TKey key]
	{
		get
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int hashCode = key.GetHashCode();
			int num = hashCode & (_entries.Length - 1);
			Entry entry = Volatile.Read(ref _entries[num]);
			if (entry != null && entry.Hash == hashCode && entry.Key.Equals(key))
			{
				return entry.Value;
			}
			if (_valueFactory == null)
			{
				throw new KeyNotFoundException();
			}
			TValue val = _valueFactory(key);
			Volatile.Write(ref _entries[num], new Entry(hashCode, key, val));
			return val;
		}
		set
		{
			Add(key, value);
		}
	}

	public CacheDict(int capacity)
	{
		_valueFactory = null;
		_entries = new Entry[(NumericHelper.PopulationCount(capacity) == 1) ? capacity : NumericHelper.NextPowerOf2(capacity)];
	}

	public CacheDict(Func<TKey, TValue> valueFactory, int capacity)
	{
		_valueFactory = valueFactory ?? throw new ArgumentNullException("valueFactory");
		_entries = new Entry[(NumericHelper.PopulationCount(capacity) == 1) ? capacity : NumericHelper.NextPowerOf2(capacity)];
	}

	public void Add(TKey key, TValue value)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		int hashCode = key.GetHashCode();
		int num = hashCode & (_entries.Length - 1);
		Entry entry = Volatile.Read(ref _entries[num]);
		if (entry == null || entry.Hash != hashCode || !entry.Key.Equals(key))
		{
			Volatile.Write(ref _entries[num], new Entry(hashCode, key, value));
		}
	}

	public bool TryGetValue(TKey key, [NotNullWhen(true)] out TValue value)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		int hashCode = key.GetHashCode();
		int num = hashCode & (_entries.Length - 1);
		Entry entry = Volatile.Read(ref _entries[num]);
		if (entry != null && entry.Hash == hashCode && entry.Key.Equals(key))
		{
			value = entry.Value;
			return value != null;
		}
		value = default(TValue);
		return false;
	}
}
