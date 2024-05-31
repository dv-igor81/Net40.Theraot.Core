using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Theraot.Collections.Specialized;

namespace System.Collections.ObjectModel;

[Serializable]
public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>
{
	[Serializable]
	public sealed class KeyCollection : ICollection<TKey>, IEnumerable<TKey>, IEnumerable, ICollection
	{
		private readonly ICollection<TKey> _wrapped;

		public int Count => _wrapped.Count;

		bool ICollection<TKey>.IsReadOnly => true;

		bool ICollection.IsSynchronized => ((ICollection)_wrapped).IsSynchronized;

		object ICollection.SyncRoot => ((ICollection)_wrapped).SyncRoot;

		internal KeyCollection(ICollection<TKey> wrapped)
		{
			_wrapped = wrapped ?? throw new ArgumentNullException("wrapped");
		}

		void ICollection<TKey>.Add(TKey item)
		{
			throw new NotSupportedException();
		}

		void ICollection<TKey>.Clear()
		{
			throw new NotSupportedException();
		}

		bool ICollection<TKey>.Contains(TKey item)
		{
			return _wrapped.Contains(item);
		}

		public void CopyTo(TKey[] array, int arrayIndex)
		{
			_wrapped.CopyTo(array, arrayIndex);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)_wrapped).CopyTo(array, index);
		}

		public IEnumerator<TKey> GetEnumerator()
		{
			return _wrapped.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		bool ICollection<TKey>.Remove(TKey item)
		{
			throw new NotSupportedException();
		}
	}

	[Serializable]
	public sealed class ValueCollection : ICollection<TValue>, IEnumerable<TValue>, IEnumerable, ICollection
	{
		private readonly ICollection<TValue> _wrapped;

		public int Count => _wrapped.Count;

		bool ICollection<TValue>.IsReadOnly => true;

		bool ICollection.IsSynchronized => ((ICollection)_wrapped).IsSynchronized;

		object ICollection.SyncRoot => ((ICollection)_wrapped).SyncRoot;

		internal ValueCollection(ICollection<TValue> wrapped)
		{
			_wrapped = wrapped ?? throw new ArgumentNullException("wrapped");
		}

		void ICollection<TValue>.Add(TValue item)
		{
			throw new NotSupportedException();
		}

		void ICollection<TValue>.Clear()
		{
			throw new NotSupportedException();
		}

		bool ICollection<TValue>.Contains(TValue item)
		{
			return _wrapped.Contains(item);
		}

		public void CopyTo(TValue[] array, int arrayIndex)
		{
			_wrapped.CopyTo(array, arrayIndex);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)_wrapped).CopyTo(array, index);
		}

		public IEnumerator<TValue> GetEnumerator()
		{
			return _wrapped.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		bool ICollection<TValue>.Remove(TValue item)
		{
			throw new NotSupportedException();
		}
	}

	public int Count => Dictionary.Count;

	bool IDictionary.IsFixedSize => ((IDictionary)Dictionary).IsFixedSize;

	bool IDictionary.IsReadOnly => true;

	bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => true;

	bool ICollection.IsSynchronized => ((ICollection)Dictionary).IsSynchronized;

	public KeyCollection Keys { get; }

	ICollection IDictionary.Keys => Keys;

	ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

	IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

	object ICollection.SyncRoot => ((ICollection)Dictionary).SyncRoot;

	public ValueCollection Values { get; }

	ICollection IDictionary.Values => Values;

	ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

	IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

	protected IDictionary<TKey, TValue> Dictionary { get; }

	public TValue this[TKey key] => Dictionary[key];

	object? IDictionary.this[object key]
	{
		get
		{
			if (key != null)
			{
				if (key is TKey key2)
				{
					return this[key2];
				}
				return null;
			}
			throw new ArgumentNullException("key");
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	TValue IDictionary<TKey, TValue>.this[TKey key]
	{
		get
		{
			return this[key];
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
	{
		Dictionary = dictionary ?? throw new ArgumentNullException("dictionary");
		Keys = new KeyCollection(new ProxyCollection<TKey>(() => Dictionary.Keys));
		Values = new ValueCollection(new ProxyCollection<TValue>(() => Dictionary.Values));
	}

	void IDictionary.Add(object key, object value)
	{
		throw new NotSupportedException();
	}

	void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
	{
		throw new NotSupportedException();
	}

	void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
	{
		throw new NotSupportedException();
	}

	void IDictionary.Clear()
	{
		throw new NotSupportedException();
	}

	void ICollection<KeyValuePair<TKey, TValue>>.Clear()
	{
		throw new NotSupportedException();
	}

	bool IDictionary.Contains(object key)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		return key is TKey key2 && ContainsKey(key2);
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
	{
		return Dictionary.Contains(item);
	}

	public bool ContainsKey(TKey key)
	{
		return Dictionary.ContainsKey(key);
	}

	void ICollection.CopyTo(Array array, int index)
	{
		((ICollection)Dictionary).CopyTo(array, index);
	}

	void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		Dictionary.CopyTo(array, arrayIndex);
	}

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		return Dictionary.GetEnumerator();
	}

	IDictionaryEnumerator IDictionary.GetEnumerator()
	{
		return ((IDictionary)Dictionary).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	void IDictionary.Remove(object key)
	{
		throw new NotSupportedException();
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
	{
		throw new NotSupportedException();
	}

	bool IDictionary<TKey, TValue>.Remove(TKey key)
	{
		throw new NotSupportedException();
	}

	public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
	{
		return Dictionary.TryGetValue(key, out value);
	}
}
