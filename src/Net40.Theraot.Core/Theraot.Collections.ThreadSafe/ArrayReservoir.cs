using System;
using Theraot.Core;

namespace Theraot.Collections.ThreadSafe;

internal static class ArrayReservoir
{
	internal const int CapacityCount = 14;

	internal const int MaxCapacity = 65536;

	internal const int MaxCapacityLog2 = 16;

	internal const int MinCapacity = 8;

	internal const int MinCapacityLog2 = 3;

	internal const int PoolSize = 16;

	private static readonly CacheDict<Type, Pool<Array>[]> _pools = new CacheDict<Type, Pool<Array>[]>(256);

	public static Pool<Array>? GetPool<T>(int index)
	{
		CacheDict<Type, Pool<Array>[]> pools = _pools;
		if (pools == null)
		{
			return null;
		}
		if (pools.TryGetValue(typeof(T), out var value))
		{
			return (value != null) ? value[index] : null;
		}
		Pool<Array>[] array2 = (pools[typeof(T)] = new Pool<Array>[14]);
		value = array2;
		PopulatePools(value);
		return value[index];
	}

	private static void PopulatePools(Pool<Array>[] poolArray)
	{
		for (int i = 0; i < 14; i++)
		{
			int currentIndex = i;
			poolArray[currentIndex] = new Pool<Array>(16, delegate(Array item)
			{
				int length = 8 << currentIndex;
				Array.Clear(item, 0, length);
			});
		}
	}
}
internal static class ArrayReservoir<T>
{
	internal static void DonateArray(T[] donation)
	{
		if (donation != null)
		{
			int num = donation.Length;
			if (num != 0 && num >= 8 && num <= 65536)
			{
				num = ((NumericHelper.PopulationCount(num) == 1) ? num : NumericHelper.NextPowerOf2(num));
				int index = NumericHelper.Log2(num) - 3;
				ArrayReservoir.GetPool<T>(index)?.Donate(donation);
			}
		}
	}

	internal static T[] GetArray(int capacity)
	{
		if (capacity == 0)
		{
			return ArrayEx.Empty<T>();
		}
		if (capacity < 8)
		{
			capacity = 8;
		}
		capacity = ((NumericHelper.PopulationCount(capacity) == 1) ? capacity : NumericHelper.NextPowerOf2(capacity));
		if (capacity > 65536)
		{
			return new T[capacity];
		}
		int index = NumericHelper.Log2(capacity) - 3;
		Pool<Array> pool = ArrayReservoir.GetPool<T>(index);
		if (pool == null)
		{
			return new T[capacity];
		}
		if (pool.TryGet(out var result))
		{
			return (T[])result;
		}
		return new T[capacity];
	}
}
