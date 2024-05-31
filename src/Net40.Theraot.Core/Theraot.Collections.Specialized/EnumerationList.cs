using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Theraot.Collections.Specialized;

[DebuggerNonUserCode]
public sealed class EnumerationList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IReadOnlyList<T>, IReadOnlyCollection<T>
{
	private readonly Func<T, bool> _contains;

	private readonly Action<T[], int> _copyTo;

	private readonly Func<int> _count;

	private readonly Func<IEnumerator<T>> _getEnumerator;

	private readonly Func<int, T> _index;

	private readonly Func<T, int> _indexOf;

	public int Count => _count();

	bool ICollection<T>.IsReadOnly => true;

	public T this[int index] => _index(index);

	T IList<T>.this[int index]
	{
		get
		{
			return this[index];
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	private EnumerationList(IEnumerable<T> wrapped)
	{
		EnumerationList<T> enumerationList = this;
		IEnumerable<T> enumerable = wrapped;
		IEnumerable<T> enumerable2 = enumerable;
		T[] array = enumerable2 as T[];
		if (array == null)
		{
			IList<T> list = enumerable2 as IList<T>;
			if (list == null)
			{
				IReadOnlyList<T> readOnlyList = enumerable2 as IReadOnlyList<T>;
				if (readOnlyList == null)
				{
					ICollection<T> collection = enumerable2 as ICollection<T>;
					if (collection == null)
					{
						IReadOnlyCollection<T> readOnlyCollection = enumerable2 as IReadOnlyCollection<T>;
						if (readOnlyCollection != null)
						{
							_count = () => readOnlyCollection.Count;
							_contains = ((IEnumerable<T>)readOnlyCollection).Contains<T>;
							_index = Index;
							_indexOf = ((IEnumerable<T>)readOnlyCollection).IndexOf<T>;
							_getEnumerator = readOnlyCollection.GetEnumerator;
							_copyTo = ((IEnumerable<T>)readOnlyCollection).CopyTo<T>;
						}
						else
						{
							_count = wrapped.Count<T>;
							_contains = wrapped.Contains<T>;
							_index = Index;
							_indexOf = wrapped.IndexOf<T>;
							_getEnumerator = wrapped.GetEnumerator;
							_copyTo = wrapped.CopyTo<T>;
						}
					}
					else
					{
						_count = () => collection.Count;
						_contains = collection.Contains;
						_index = Index;
						_indexOf = ((IEnumerable<T>)collection).IndexOf<T>;
						_getEnumerator = collection.GetEnumerator;
						_copyTo = collection.CopyTo;
					}
				}
				else
				{
					_count = () => readOnlyList.Count;
					_contains = ((IEnumerable<T>)readOnlyList).Contains<T>;
					_index = (int index) => readOnlyList[index];
					_indexOf = ((IEnumerable<T>)readOnlyList).IndexOf<T>;
					_getEnumerator = readOnlyList.GetEnumerator;
					_copyTo = ((IEnumerable<T>)readOnlyList).CopyTo<T>;
				}
			}
			else
			{
				_count = () => list.Count;
				_contains = list.Contains;
				_index = (int index) => list[index];
				_indexOf = list.IndexOf;
				_getEnumerator = list.GetEnumerator;
				_copyTo = list.CopyTo;
			}
		}
		else
		{
			_count = () => array.Length;
			_contains = (T item) => Array.IndexOf(array, item) != -1;
			_index = (int index) => array[index];
			_indexOf = (T item) => Array.IndexOf(array, item);
			_getEnumerator = ((IEnumerable<T>)array).GetEnumerator;
			_copyTo = array.CopyTo;
		}
		T Index(int index)
		{
			if (index >= enumerationList._count())
			{
				throw new ArgumentOutOfRangeException("index");
			}
			using (IEnumerator<T> enumerator = wrapped.Skip(index).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
			}
			throw new ArgumentOutOfRangeException("index");
		}
	}

	public static EnumerationList<T> Create(IEnumerable<T> wrapped)
	{
		if (wrapped == null)
		{
			throw new ArgumentNullException("wrapped");
		}
		return new EnumerationList<T>(wrapped);
	}

	void ICollection<T>.Add(T item)
	{
		throw new NotSupportedException();
	}

	void ICollection<T>.Clear()
	{
		throw new NotSupportedException();
	}

	public bool Contains(T item)
	{
		return _contains(item);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		_copyTo(array, arrayIndex);
	}

	public IEnumerator<T> GetEnumerator()
	{
		return _getEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public int IndexOf(T item)
	{
		return _indexOf(item);
	}

	void IList<T>.Insert(int index, T item)
	{
		throw new NotSupportedException();
	}

	bool ICollection<T>.Remove(T item)
	{
		throw new NotSupportedException();
	}

	void IList<T>.RemoveAt(int index)
	{
		throw new NotSupportedException();
	}
}
