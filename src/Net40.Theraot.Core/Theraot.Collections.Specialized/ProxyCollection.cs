using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Theraot.Collections.Specialized;

[DebuggerNonUserCode]
public sealed class ProxyCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable
{
	private readonly Func<ICollection<T>> _wrapped;

	public IReadOnlyCollection<T> AsIReadOnlyCollection { get; }

	public ICollection<T> AsReadOnlyICollection { get; }

	public int Count => Instance.Count;

	public bool IsReadOnly => Instance.IsReadOnly;

	private ICollection<T> Instance => _wrapped() ?? ArrayEx.Empty<T>();

	public ProxyCollection(Func<ICollection<T>> wrapped)
	{
		_wrapped = wrapped ?? throw new ArgumentNullException("wrapped");
		AsReadOnlyICollection = (ICollection<T>)(AsIReadOnlyCollection = EnumerationList<T>.Create(this));
	}

	public void Add(T item)
	{
		Instance.Add(item);
	}

	public void Clear()
	{
		Instance.Clear();
	}

	public bool Contains(T item)
	{
		return Instance.Contains(item);
	}

	public void CopyTo(T[] array)
	{
		Instance.CopyTo(array, 0);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		Instance.CopyTo(array, arrayIndex);
	}

	public void CopyTo(T[] array, int arrayIndex, int countLimit)
	{
		Extensions.CanCopyTo(array, arrayIndex, countLimit);
		Instance.CopyTo(array, arrayIndex, countLimit);
	}

	public IEnumerator<T> GetEnumerator()
	{
		ICollection<T> collection = Instance;
		foreach (T item in collection)
		{
			if (collection != Instance)
			{
				throw new InvalidOperationException();
			}
			yield return item;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public bool Remove(T item)
	{
		return Instance.Remove(item);
	}

	public bool Remove(T item, IEqualityComparer<T> comparer)
	{
		if (comparer == null)
		{
			comparer = EqualityComparer<T>.Default;
		}
		return Instance.RemoveWhereEnumerable((T input) => comparer.Equals(input, item)).Any();
	}

	public T[] ToArray()
	{
		T[] array = new T[Instance.Count];
		Instance.CopyTo(array, 0);
		return array;
	}
}
[DebuggerNonUserCode]
public sealed class ProxyCollection<TCovered, TUncovered> : ICollection<TUncovered>, IEnumerable<TUncovered>, IEnumerable
{
	private readonly Func<TUncovered, TCovered> _cover;

	private readonly Func<TCovered, TUncovered> _uncover;

	private readonly Func<ICollection<TCovered>> _wrapped;

	public IReadOnlyCollection<TUncovered> AsIReadOnlyCollection { get; }

	public ICollection<TUncovered> AsReadOnlyICollection { get; }

	public int Count => Instance.Count;

	public bool IsReadOnly => Instance.IsReadOnly;

	private ICollection<TCovered> Instance => _wrapped() ?? ArrayEx.Empty<TCovered>();

	public ProxyCollection(Func<ICollection<TCovered>> wrapped, Func<TCovered, TUncovered> uncover, Func<TUncovered, TCovered> cover)
	{
		_uncover = uncover ?? throw new ArgumentNullException("uncover");
		_cover = cover ?? throw new ArgumentNullException("cover");
		_wrapped = wrapped ?? throw new ArgumentNullException("wrapped");
		AsReadOnlyICollection = (ICollection<TUncovered>)(AsIReadOnlyCollection = EnumerationList<TUncovered>.Create(this));
	}

	public void Add(TUncovered item)
	{
		Instance.Add(_cover(item));
	}

	public void Clear()
	{
		Instance.Clear();
	}

	public bool Contains(TUncovered item)
	{
		return Instance.Contains(_cover(item));
	}

	public void CopyTo(TUncovered[] array)
	{
		Extensions.CanCopyTo(Count, array);
		Instance.ConvertedCopyTo(_uncover, array, 0);
	}

	public void CopyTo(TUncovered[] array, int arrayIndex)
	{
		Extensions.CanCopyTo(Count, array, arrayIndex);
		Instance.ConvertedCopyTo(_uncover, array, arrayIndex);
	}

	public void CopyTo(TUncovered[] array, int arrayIndex, int countLimit)
	{
		Extensions.CanCopyTo(array, arrayIndex, countLimit);
		Instance.ConvertedCopyTo(_uncover, array, arrayIndex, countLimit);
	}

	public IEnumerator<TUncovered> GetEnumerator()
	{
		ICollection<TCovered> collection = Instance;
		foreach (TCovered item in collection)
		{
			if (collection != Instance)
			{
				throw new InvalidOperationException();
			}
			yield return _uncover(item);
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public bool Remove(TUncovered item)
	{
		return Instance.Remove(_cover(item));
	}

	public bool Remove(TUncovered item, IEqualityComparer<TUncovered> comparer)
	{
		if (comparer == null)
		{
			comparer = EqualityComparer<TUncovered>.Default;
		}
		return Instance.RemoveWhereEnumerable((TCovered input) => comparer.Equals(_uncover(input), item)).Any();
	}

	public TUncovered[] ToArray()
	{
		TUncovered[] array = new TUncovered[Instance.Count];
		Instance.ConvertedCopyTo(_uncover, array, 0);
		return array;
	}
}