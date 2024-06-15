using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Theraot.Collections.Specialized;

namespace Theraot.Collections.ThreadSafe;

public sealed class ThreadSafeCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IHasComparer<T>
{
	private int _maxIndex;

	private Bucket<T> _wrapped;

	public IEqualityComparer<T> Comparer { get; }

	public int Count => _wrapped.Count;

	bool ICollection<T>.IsReadOnly => false;

	public ThreadSafeCollection()
		: this((IEqualityComparer<T>)EqualityComparer<T>.Default)
	{
	}

	public ThreadSafeCollection(IEqualityComparer<T> comparer)
	{
		_maxIndex = -1;
		Comparer = comparer ?? EqualityComparer<T>.Default;
		_wrapped = new Bucket<T>();
	}

	public void Add(T item)
	{
		_wrapped.Set(Interlocked.Increment(ref _maxIndex), item);
	}

	public void Clear()
	{
		_wrapped = new Bucket<T>();
	}

	public IEnumerable<T> ClearEnumerable()
	{
		Bucket<T> bucket = new Bucket<T>();
		Interlocked.Exchange(ref _wrapped, bucket);
		return bucket;
	}

	public bool Contains(Predicate<T> itemCheck)
	{
		return _wrapped.Where(itemCheck).Any();
	}

	public bool Contains(T item)
	{
		return _wrapped.Where(Check).Any();
		bool Check(T input)
		{
			return Comparer.Equals(input, item);
		}
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		Extensions.CanCopyTo(Count, array, arrayIndex);
		Extensions.CopyTo(_wrapped, array, arrayIndex);
	}

	public IEnumerator<T> GetEnumerator()
	{
		return _wrapped.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public bool Remove(T item)
	{
		return _wrapped.RemoveWhereEnumerable(Check).Any();
		bool Check(T input)
		{
			return Comparer.Equals(input, item);
		}
	}

	public int RemoveWhere(Predicate<T> check)
	{
		return _wrapped.RemoveWhere(check);
	}

	public IEnumerable<T> RemoveWhereEnumerable(Predicate<T> check)
	{
		return _wrapped.RemoveWhereEnumerable(check);
	}

	public IEnumerable<T> Where(Predicate<T> check)
	{
		return _wrapped.Where(check);
	}
}