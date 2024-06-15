using System;
using System.Collections.Generic;
using System.Linq;

namespace Theraot.Collections.ThreadSafe;

public static class BucketHelper
{
	internal static object Null { get; }

	static BucketHelper()
	{
		Null = new object();
	}

	public static void InsertOrUpdate<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, Func<T, T> itemUpdateFactory, out bool isNew)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		if (itemFactory == null)
		{
			throw new ArgumentNullException("itemFactory");
		}
		if (itemUpdateFactory == null)
		{
			throw new ArgumentNullException("itemUpdateFactory");
		}
		isNew = true;
		bool flag = false;
		T item = default(T);
		while (true)
		{
			if (isNew)
			{
				if (!flag)
				{
					item = itemFactory();
					flag = true;
				}
				if (bucket.Insert(index, item, out var _))
				{
					break;
				}
				isNew = false;
			}
			else if (bucket.Update(index, itemUpdateFactory, Tautology, out isNew))
			{
				break;
			}
		}
	}

	public static bool InsertOrUpdateChecked<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, Func<T, T> itemUpdateFactory, Predicate<T> check, out bool isNew)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		if (itemFactory == null)
		{
			throw new ArgumentNullException("itemFactory");
		}
		if (itemUpdateFactory == null)
		{
			throw new ArgumentNullException("itemUpdateFactory");
		}
		if (check == null)
		{
			throw new ArgumentNullException("check");
		}
		isNew = true;
		bool flag = false;
		T item = default(T);
		while (true)
		{
			if (isNew)
			{
				if (!flag)
				{
					item = itemFactory();
					flag = true;
				}
				if (bucket.Insert(index, item, out var _))
				{
					return true;
				}
				isNew = false;
			}
			else
			{
				if (bucket.Update(index, itemUpdateFactory, check, out isNew))
				{
					return true;
				}
				if (!isNew)
				{
					break;
				}
			}
		}
		return false;
	}

	public static bool InsertOrUpdateChecked<T>(this IBucket<T> bucket, int index, T item, Func<T, T> itemUpdateFactory, Predicate<T> check, out bool isNew)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		if (itemUpdateFactory == null)
		{
			throw new ArgumentNullException("itemUpdateFactory");
		}
		if (check == null)
		{
			throw new ArgumentNullException("check");
		}
		isNew = true;
		while (true)
		{
			if (isNew)
			{
				if (bucket.Insert(index, item, out var _))
				{
					return true;
				}
				isNew = false;
				continue;
			}
			if (bucket.Update(index, itemUpdateFactory, check, out isNew))
			{
				return true;
			}
			if (!isNew)
			{
				break;
			}
		}
		return false;
	}

	public static bool InsertOrUpdateChecked<T>(this IBucket<T> bucket, int index, T item, Predicate<T> check, out bool isNew)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		if (check == null)
		{
			throw new ArgumentNullException("check");
		}
		isNew = true;
		while (true)
		{
			if (isNew)
			{
				if (bucket.Insert(index, item, out var _))
				{
					return true;
				}
				isNew = false;
				continue;
			}
			if (bucket.Update(index, (T _) => item, check, out isNew))
			{
				return true;
			}
			if (!isNew)
			{
				break;
			}
		}
		return false;
	}

	public static int RemoveWhere<T>(this IBucket<T> bucket, Predicate<T> check)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		if (check == null)
		{
			throw new ArgumentNullException("check");
		}
		IEnumerable<KeyValuePair<int, T>> source = bucket.WhereIndexed(check);
		return source.Count((KeyValuePair<int, T> pair) => bucket.RemoveAt(pair.Key));
	}

	public static IEnumerable<T> RemoveWhereEnumerable<T>(this IBucket<T> bucket, Predicate<T> check)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		if (check == null)
		{
			throw new ArgumentNullException("check");
		}
		IEnumerable<KeyValuePair<int, T>> source = bucket.WhereIndexed(check);
		return source.Where(delegate(KeyValuePair<int, T> pair)
		{
			IBucket<T> bucket2 = bucket;
			KeyValuePair<int, T> keyValuePair2 = pair;
			return bucket2.RemoveAt(keyValuePair2.Key);
		}).Select(delegate(KeyValuePair<int, T> pair)
		{
			KeyValuePair<int, T> keyValuePair = pair;
			return keyValuePair.Value;
		});
	}

	public static void Set<T>(this IBucket<T> bucket, int index, T value)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		bucket.Set(index, value, out var _);
	}

	public static bool TryGetOrInsert<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, out T stored)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		if (bucket.TryGet(index, out stored))
		{
			return false;
		}
		if (itemFactory == null)
		{
			throw new ArgumentNullException("itemFactory");
		}
		T val = itemFactory();
		if (!bucket.Insert(index, val, out stored))
		{
			return false;
		}
		stored = val;
		return true;
	}

	public static bool TryGetOrInsert<T>(this IBucket<T> bucket, int index, T item, out T stored)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		if (bucket.Insert(index, item, out var previous))
		{
			stored = item;
			return true;
		}
		stored = previous;
		return false;
	}

	public static bool UpdateChecked<T>(this IBucket<T> bucket, int index, T item, Predicate<T> check)
	{
		if (bucket == null)
		{
			throw new ArgumentNullException("bucket");
		}
		bool isEmpty;
		return bucket.Update(index, (T _) => item, check, out isEmpty);
	}

	private static bool Tautology<T>(T item)
	{
		return true;
	}
}