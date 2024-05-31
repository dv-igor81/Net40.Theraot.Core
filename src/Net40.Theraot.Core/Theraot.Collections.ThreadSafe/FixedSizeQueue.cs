using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Theraot.Core;

namespace Theraot.Collections.ThreadSafe;

[Serializable]
public sealed class FixedSizeQueue<T> : IProducerConsumerCollection<T>, IEnumerable<T>, IEnumerable, ICollection
{
	private readonly FixedSizeBucket<T> _entries;

	private int _indexDequeue;

	private int _indexEnqueue;

	private int _preCount;

	public int Capacity { get; }

	public int Count => _entries.Count;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public FixedSizeQueue(IEnumerable<T> source)
	{
		_indexDequeue = 0;
		_entries = new FixedSizeBucket<T>(source);
		Capacity = _entries.Capacity;
		_indexEnqueue = _entries.Count;
		_preCount = _indexEnqueue;
	}

	public FixedSizeQueue(int capacity)
	{
		Capacity = ((NumericHelper.PopulationCount(capacity) == 1) ? capacity : NumericHelper.NextPowerOf2(capacity));
		_preCount = 0;
		_indexEnqueue = 0;
		_indexDequeue = 0;
		_entries = new FixedSizeBucket<T>(Capacity);
	}

	public void CopyTo(T[] array, int index)
	{
		_entries.CopyTo(array, index);
	}

	void ICollection.CopyTo(Array array, int index)
	{
		Extensions.CanCopyTo(Count, array, index);
		this.DeprecatedCopyTo(array, index);
	}

	public IEnumerator<T> GetEnumerator()
	{
		return _entries.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public T Peek()
	{
		int num = Interlocked.Add(ref _indexEnqueue, 0);
		if (num < Capacity && num > 0 && _entries.TryGet(num, out var value))
		{
			return value;
		}
		throw new InvalidOperationException("Empty");
	}

	public T[] ToArray()
	{
		return this.ToArray(Count);
	}

	public bool TryAdd(T item)
	{
		if (_entries.Count >= Capacity)
		{
			return false;
		}
		int num = Interlocked.Increment(ref _preCount);
		if (num <= Capacity)
		{
			int index = (Interlocked.Increment(ref _indexEnqueue) - 1) & (Capacity - 1);
			if (_entries.InsertInternal(index, item))
			{
				return true;
			}
		}
		Interlocked.Decrement(ref _preCount);
		return false;
	}

	public bool TryPeek(out T item)
	{
		item = default(T);
		int num = Interlocked.Add(ref _indexDequeue, 0);
		return num < Capacity && num > 0 && _entries.TryGetInternal(num, out item);
	}

	public bool TryTake(out T item)
	{
		if (_entries.Count > 0)
		{
			int num = Interlocked.Decrement(ref _preCount);
			if (num >= 0)
			{
				int index = (Interlocked.Increment(ref _indexDequeue) - 1) & (Capacity - 1);
				if (_entries.RemoveAtInternal(index, out item))
				{
					return true;
				}
			}
			Interlocked.Increment(ref _preCount);
		}
		item = default(T);
		return false;
	}
}
