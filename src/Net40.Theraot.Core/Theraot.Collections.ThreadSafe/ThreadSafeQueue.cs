using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Theraot.Core;

namespace Theraot.Collections.ThreadSafe;

[Serializable]
public sealed class ThreadSafeQueue<T> : IProducerConsumerCollection<T>, IEnumerable<T>, IEnumerable, ICollection
{
	private int _count;

	private Node<FixedSizeQueue<T>> _root;

	private Node<FixedSizeQueue<T>> _tail;

	public int Count => Volatile.Read(ref _count);

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public ThreadSafeQueue()
	{
		_root = Node<FixedSizeQueue<T>>.GetNode(null, new FixedSizeQueue<T>(64));
		_tail = _root;
	}

	public ThreadSafeQueue(IEnumerable<T> source)
	{
		_root = Node<FixedSizeQueue<T>>.GetNode(null, new FixedSizeQueue<T>(source));
		_count = _root.Value.Count;
		_tail = _root;
	}

	public void Add(T item)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			Node<FixedSizeQueue<T>> node = Volatile.Read(ref _tail);
			if (node.Value.TryAdd(item))
			{
				break;
			}
			Node<FixedSizeQueue<T>> node2 = Node<FixedSizeQueue<T>>.GetNode(null, new FixedSizeQueue<T>(64));
			Node<FixedSizeQueue<T>> node3 = Interlocked.CompareExchange(ref node.Link, node2, null);
			if (node3 == null)
			{
				Volatile.Write(ref _tail, node2);
			}
			spinWait.SpinOnce();
		}
		Interlocked.Increment(ref _count);
	}

	public void CopyTo(T[] array, int index)
	{
		Extensions.CanCopyTo(Count, array, index);
		this.DeprecatedCopyTo(array, index);
	}

	void ICollection.CopyTo(Array array, int index)
	{
		Extensions.CanCopyTo(Count, array, index);
		this.DeprecatedCopyTo(array, index);
	}

	public IEnumerator<T> GetEnumerator()
	{
		foreach (Node<FixedSizeQueue<T>> root in SequenceHelper.ExploreSequenceUntilNull(_root, (Node<FixedSizeQueue<T>> found) => found.Link))
		{
			foreach (T item in root.Value)
			{
				yield return item;
			}
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public T[] ToArray()
	{
		return this.ToArray(Count);
	}

	bool IProducerConsumerCollection<T>.TryAdd(T item)
	{
		Add(item);
		return true;
	}

	public bool TryPeek(out T item)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			Node<FixedSizeQueue<T>> node = Volatile.Read(ref _root);
			if (node.Value.TryPeek(out item))
			{
				return true;
			}
			if (node.Link == null)
			{
				break;
			}
			Node<FixedSizeQueue<T>> node2 = Interlocked.CompareExchange(ref _root, node.Link, node);
			if (node2 == node)
			{
				Node<FixedSizeQueue<T>>.Donate(node);
			}
			spinWait.SpinOnce();
		}
		return false;
	}

	public bool TryTake(out T item)
	{
		SpinWait spinWait = default(SpinWait);
		while (true)
		{
			Node<FixedSizeQueue<T>> node = Volatile.Read(ref _root);
			if (node.Value.TryTake(out item))
			{
				Interlocked.Decrement(ref _count);
				return true;
			}
			if (node.Link == null)
			{
				break;
			}
			Node<FixedSizeQueue<T>> node2 = Interlocked.CompareExchange(ref _root, node.Link, node);
			if (node2 == node)
			{
				Node<FixedSizeQueue<T>>.Donate(node);
			}
			spinWait.SpinOnce();
		}
		return false;
	}
}
