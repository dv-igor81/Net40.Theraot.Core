using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace Theraot.Collections.ThreadSafe;

[Serializable]
public sealed class Bucket<T> : IBucket<T>, IEnumerable<T>, IEnumerable
{
	private readonly BucketCore _bucketCore;

	private int _count;

	public int Count => _count;

	public Bucket()
	{
		_bucketCore = new BucketCore();
	}

	public Bucket(IEnumerable<T> source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		_bucketCore = new BucketCore();
		int num = 0;
		foreach (T item in source)
		{
			Insert(num, item);
			num++;
		}
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		Extensions.CanCopyTo(Count, array, arrayIndex);
		Extensions.CopyTo(this, array, arrayIndex);
	}

	public IEnumerable<T> EnumerateRange(int indexFrom, int indexTo)
	{
		IEnumerable<object> source = _bucketCore.EnumerateRange(indexFrom, indexTo);
		return source.Select(Selector);
		static T Selector(object value)
		{
			return (value == BucketHelper.Null) ? default(T) : ((T)value);
		}
	}

	public bool Exchange(int index, T item, [MaybeNullWhen(true)] out T previous)
	{
		object found = BucketHelper.Null;
		previous = default(T);
		if (_bucketCore.DoMayIncrement(index, delegate(ref object target)
		{
			T val = item;
			found = Interlocked.Exchange(ref target, (val != null) ? ((object)val) : BucketHelper.Null);
			return found == null;
		}))
		{
			Interlocked.Increment(ref _count);
			return true;
		}
		if (found != BucketHelper.Null)
		{
			previous = (T)found;
		}
		return false;
	}

	public IEnumerator<T> GetEnumerator()
	{
		foreach (object value in _bucketCore)
		{
			yield return (value == BucketHelper.Null) ? default(T) : ((T)value);
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public bool Insert(int index, T item)
	{
		bool flag = _bucketCore.DoMayIncrement(index, delegate(ref object target)
		{
			T val = item;
			object obj = Interlocked.CompareExchange(ref target, (val != null) ? ((object)val) : BucketHelper.Null, null);
			return obj == null;
		});
		if (flag)
		{
			Interlocked.Increment(ref _count);
		}
		return flag;
	}

	public bool Insert(int index, T item, out T previous)
	{
		object found = BucketHelper.Null;
		previous = default(T);
		if (_bucketCore.DoMayIncrement(index, delegate(ref object target)
		{
			T val = item;
			found = Interlocked.CompareExchange(ref target, (val != null) ? ((object)val) : BucketHelper.Null, null);
			return found == null;
		}))
		{
			Interlocked.Increment(ref _count);
			return true;
		}
		if (found != BucketHelper.Null)
		{
			previous = (T)found;
		}
		return false;
	}

	public bool RemoveAt(int index)
	{
		bool flag = _bucketCore.DoMayDecrement(index, delegate(ref object target)
		{
			return Interlocked.Exchange(ref target, null) != null;
		});
		if (flag)
		{
			Interlocked.Decrement(ref _count);
		}
		return flag;
	}

	public bool RemoveAt(int index, out T previous)
	{
		object found = BucketHelper.Null;
		previous = default(T);
		if (!_bucketCore.DoMayDecrement(index, delegate(ref object target)
		{
			found = Interlocked.Exchange(ref target, null);
			return found != null;
		}))
		{
			return false;
		}
		Interlocked.Decrement(ref _count);
		if (found != BucketHelper.Null)
		{
			previous = (T)found;
		}
		return true;
	}

	public bool RemoveAt(int index, Predicate<T> check)
	{
		if (check == null)
		{
			throw new ArgumentNullException("check");
		}
		return _bucketCore.DoMayDecrement(index, delegate(ref object target)
		{
			object obj = Interlocked.CompareExchange(ref target, null, null);
			if (obj == null)
			{
				return false;
			}
			T obj2 = ((obj == BucketHelper.Null) ? default(T) : ((T)obj));
			if (!check(obj2))
			{
				return false;
			}
			object obj3 = Interlocked.CompareExchange(ref target, null, obj);
			if (obj != obj3)
			{
				return false;
			}
			Interlocked.Decrement(ref _count);
			return true;
		});
	}

	public void Set(int index, T item, out bool isNew)
	{
		isNew = _bucketCore.DoMayIncrement(index, delegate(ref object target)
		{
			T val = item;
			return Interlocked.Exchange(ref target, (val != null) ? ((object)val) : BucketHelper.Null) == null;
		});
		if (isNew)
		{
			Interlocked.Increment(ref _count);
		}
	}

	public bool TryGet(int index, out T value)
	{
		object found = BucketHelper.Null;
		value = default(T);
		if (!_bucketCore.Do(index, delegate(ref object target)
		{
			found = Interlocked.CompareExchange(ref target, null, null);
			return true;
		}) || found == null)
		{
			return false;
		}
		if (found != BucketHelper.Null)
		{
			value = (T)found;
		}
		return true;
	}

	public bool Update(int index, Func<T, T> itemUpdateFactory, Predicate<T> check, out bool isEmpty)
	{
		if (itemUpdateFactory == null)
		{
			throw new ArgumentNullException("itemUpdateFactory");
		}
		if (check == null)
		{
			throw new ArgumentNullException("check");
		}
		object found = BucketHelper.Null;
		object compare = BucketHelper.Null;
		bool result = false;
		if (!_bucketCore.Do(index, delegate(ref object target)
		{
			found = Interlocked.CompareExchange(ref target, null, null);
			if (found == null)
			{
				return true;
			}
			T val = ((found == BucketHelper.Null) ? default(T) : ((T)found));
			if (!check(val))
			{
				return true;
			}
			T val2 = itemUpdateFactory(val);
			T val3 = val2;
			compare = Interlocked.CompareExchange(ref target, (val3 != null) ? ((object)val3) : BucketHelper.Null, found);
			result = found == compare;
			return true;
		}))
		{
			isEmpty = true;
			return false;
		}
		isEmpty = found == null || compare == null;
		return result;
	}

	public IEnumerable<T> Where(Predicate<T> check)
	{
		if (check == null)
		{
			throw new ArgumentNullException("check");
		}
		return WhereExtracted();
		IEnumerable<T> WhereExtracted()
		{
			foreach (object value in _bucketCore)
			{
				T castValue = ((value == BucketHelper.Null) ? default(T) : ((T)value));
				if (check(castValue))
				{
					yield return castValue;
				}
			}
		}
	}

	public IEnumerable<KeyValuePair<int, T>> WhereIndexed(Predicate<T> check)
	{
		if (check == null)
		{
			throw new ArgumentNullException("check");
		}
		return WhereExtracted();
		IEnumerable<KeyValuePair<int, T>> WhereExtracted()
		{
			int index = 0;
			foreach (object value in _bucketCore)
			{
				T castValue = ((value == BucketHelper.Null) ? default(T) : ((T)value));
				if (check(castValue))
				{
					yield return new KeyValuePair<int, T>(index, castValue);
					index++;
				}
			}
		}
	}
}