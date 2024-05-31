using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using Theraot.Core;
using Theraot.Threading;

namespace Theraot.Collections.ThreadSafe;

[Serializable]
internal sealed class BucketCore : IEnumerable<object>, IEnumerable, ISerializable
{
	private const int _capacity = 256;

	private const int _capacityLog2 = 8;

	private const int _mask = 255;

	private const int _maxLevel = 4;

	private readonly Func<object> _childFactory;

	private readonly int _level;

	private object?[]? _arrayFirst;

	private object?[]? _arraySecond;

	private int[]? _arrayUse;

	public BucketCore()
		: this(4)
	{
	}

	private BucketCore(SerializationInfo info, StreamingContext context)
	{
		if (!(info.GetValue("childFactory", typeof(Func<object>)) is Func<object> childFactory) || !(info.GetValue("level", typeof(int)) is int level) || !(info.GetValue("contents", typeof(object[])) is object[] array))
		{
			throw new SerializationException();
		}
		_childFactory = childFactory;
		_level = level;
		_arrayFirst = ArrayReservoir<object>.GetArray(256);
		_arraySecond = ArrayReservoir<object>.GetArray(256);
		_arrayUse = ArrayReservoir<int>.GetArray(256);
		for (int i = 0; i < Math.Min(256, array.Length); i++)
		{
			_arrayFirst[i] = array[i];
			_arraySecond[i] = array[i];
			_arrayUse[i] = 1;
		}
	}

	private BucketCore(int level)
	{
		_childFactory = ((level == 1) ? FuncHelper.GetDefaultFunc<object>() : ((Func<object>)(() => new BucketCore(_level - 1))));
		_level = level;
		_arrayFirst = ArrayReservoir<object>.GetArray(256);
		_arraySecond = ArrayReservoir<object>.GetArray(256);
		_arrayUse = ArrayReservoir<int>.GetArray(256);
	}

	~BucketCore()
	{
		if (!GCMonitor.FinalizingForUnload)
		{
			object[] arrayFirst = _arrayFirst;
			if (arrayFirst != null)
			{
				ArrayReservoir<object>.DonateArray(arrayFirst);
				_arrayFirst = null;
			}
			object[] arraySecond = _arraySecond;
			if (arraySecond != null)
			{
				ArrayReservoir<object>.DonateArray(arraySecond);
				_arraySecond = null;
			}
			int[] arrayUse = _arrayUse;
			if (arrayUse != null)
			{
				ArrayReservoir<int>.DonateArray(arrayUse);
				_arrayUse = null;
			}
		}
	}

	public bool Do(int index, DoAction callback)
	{
		object[] array = Volatile.Read(ref _arrayFirst);
		object[] array2 = Volatile.Read(ref _arraySecond);
		int[] array3 = Volatile.Read(ref _arrayUse);
		if (array == null || array2 == null || array3 == null)
		{
			return false;
		}
		int num = SubIndex(index);
		return Do(ref array3[num], ref array[num], ref array2[num], (_level == 1) ? callback : ((DoAction)delegate(ref object? target)
		{
			return target is BucketCore bucketCore && bucketCore.Do(index, callback);
		}));
	}

	public bool DoMayDecrement(int index, DoAction callback)
	{
		object[] array = Volatile.Read(ref _arrayFirst);
		object[] array2 = Volatile.Read(ref _arraySecond);
		int[] array3 = Volatile.Read(ref _arrayUse);
		if (array == null || array2 == null || array3 == null)
		{
			return false;
		}
		int num = SubIndex(index);
		return DoMayDecrement(ref array3[num], ref array[num], ref array2[num], (_level == 1) ? callback : ((DoAction)delegate(ref object? target)
		{
			return target is BucketCore bucketCore && bucketCore.DoMayDecrement(index, callback);
		}));
	}

	public bool DoMayIncrement(int index, DoAction callback)
	{
		object[] array = Volatile.Read(ref _arrayFirst);
		object[] array2 = Volatile.Read(ref _arraySecond);
		int[] array3 = Volatile.Read(ref _arrayUse);
		Func<object> childFactory = _childFactory;
		if (array == null || array2 == null || array3 == null || childFactory == null)
		{
			return false;
		}
		int num = SubIndex(index);
		return DoMayIncrement(ref array3[num], ref array[num], ref array2[num], childFactory, (_level == 1) ? callback : ((DoAction)delegate(ref object? target)
		{
			return target is BucketCore bucketCore && bucketCore.DoMayIncrement(index, callback);
		}));
	}

	public IEnumerable<object> EnumerateRange(int indexFrom, int indexTo)
	{
		if (indexFrom < 0)
		{
			throw new ArgumentOutOfRangeException("indexFrom", "indexFrom < 0");
		}
		if (indexTo < 0)
		{
			throw new ArgumentOutOfRangeException("indexTo", "indexTo < 0");
		}
		int startSubIndex = SubIndex(indexFrom);
		int endSubIndex = SubIndex(indexTo);
		return PrivateEnumerableRange(indexFrom, indexTo, startSubIndex, endSubIndex);
	}

	public IEnumerator<object> GetEnumerator()
	{
		object[] arrayFirst = Volatile.Read(ref _arrayFirst);
		object[] arraySecond = Volatile.Read(ref _arraySecond);
		int[] arrayUse = Volatile.Read(ref _arrayUse);
		if (arrayFirst == null || arraySecond == null || arrayUse == null)
		{
			return Empty();
		}
		return GetEnumeratorExtracted();
		static IEnumerator<object> Empty()
		{
			yield break;
		}
		IEnumerator<object> GetEnumeratorExtracted()
		{
			for (int subIndex = 0; subIndex < 256; subIndex++)
			{
				object foundFirst = Interlocked.CompareExchange(ref arrayFirst[subIndex], null, null);
				if (foundFirst != null)
				{
					try
					{
						Interlocked.Increment(ref arrayUse[subIndex]);
						if (_level == 1)
						{
							yield return foundFirst;
						}
						else
						{
							foreach (object item in (BucketCore)foundFirst)
							{
								yield return item;
							}
						}
					}
					finally
					{
						DoLeave(ref arrayUse[subIndex], ref arrayFirst[subIndex], ref arraySecond[subIndex]);
					}
				}
			}
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("level", _level, typeof(int));
		info.AddValue("childFactory", _childFactory, typeof(Func<object>));
		info.AddValue("contents", _arrayFirst, typeof(object[]));
	}

	private static bool Do(ref int use, ref object? first, ref object? second, DoAction callback)
	{
		try
		{
			Interlocked.Increment(ref use);
			return callback(ref first);
		}
		finally
		{
			DoLeave(ref use, ref first, ref second);
		}
	}

	private static void DoEnsureSize(ref int use, ref object? first, ref object? second, Func<object> factory)
	{
		try
		{
			Interlocked.Increment(ref use);
			object obj = Interlocked.CompareExchange(ref first, null, null);
			object obj2 = Interlocked.CompareExchange(ref second, obj, null);
			if (obj2 != null || obj != null)
			{
				return;
			}
			object obj3 = factory();
			obj = Interlocked.CompareExchange(ref first, obj3, null);
			if (obj == null)
			{
				if (obj3 != null)
				{
					Interlocked.Increment(ref use);
				}
				Interlocked.CompareExchange(ref second, obj3, null);
			}
		}
		finally
		{
			DoLeave(ref use, ref first, ref second);
		}
	}

	private static void DoLeave(ref int use, ref object? first, ref object? second)
	{
		if (Interlocked.Decrement(ref use) == 0)
		{
			Interlocked.Exchange(ref second, null);
			Interlocked.Exchange(ref first, null);
			object value = Interlocked.CompareExchange(ref second, null, null);
			Interlocked.CompareExchange(ref first, value, null);
		}
	}

	private static bool DoMayDecrement(ref int use, ref object? first, ref object? second, DoAction callback)
	{
		try
		{
			Interlocked.Increment(ref use);
			object value = Interlocked.CompareExchange(ref first, null, null);
			Interlocked.CompareExchange(ref second, value, null);
			if (!callback(ref second))
			{
				return false;
			}
			Interlocked.Decrement(ref use);
			return true;
		}
		finally
		{
			DoLeave(ref use, ref first, ref second);
		}
	}

	private static bool DoMayIncrement(ref int use, ref object? first, ref object? second, Func<object> factory, DoAction callback)
	{
		try
		{
			Interlocked.Increment(ref use);
			DoEnsureSize(ref use, ref first, ref second, factory);
			if (!callback(ref first))
			{
				return false;
			}
			Interlocked.Increment(ref use);
			return true;
		}
		finally
		{
			DoLeave(ref use, ref first, ref second);
		}
	}

	private IEnumerable<object> PrivateEnumerableRange(int indexFrom, int indexTo, int startSubIndex, int endSubIndex)
	{
		object[] arrayFirst = Volatile.Read(ref _arrayFirst);
		object[] arraySecond = Volatile.Read(ref _arraySecond);
		int[] arrayUse = Volatile.Read(ref _arrayUse);
		int step = ((endSubIndex - startSubIndex >= 0) ? 1 : (-1));
		for (int subIndex = startSubIndex; subIndex < endSubIndex + 1; subIndex += step)
		{
			try
			{
				Interlocked.Increment(ref arrayUse[subIndex]);
				object foundFirst = Interlocked.CompareExchange(ref arrayFirst[subIndex], null, null);
				if (_level == 1)
				{
					if (foundFirst != null)
					{
						yield return foundFirst;
					}
				}
				else
				{
					if (!(foundFirst is BucketCore core))
					{
						continue;
					}
					int subIndexFrom = ((subIndex == startSubIndex) ? core.SubIndex(indexFrom) : 0);
					int subIndexTo = ((subIndex == endSubIndex) ? core.SubIndex(indexTo) : 255);
					foreach (object item in core.PrivateEnumerableRange(indexFrom, indexTo, subIndexFrom, subIndexTo))
					{
						yield return item;
					}
					continue;
				}
			}
			finally
			{
				DoLeave(ref arrayUse[subIndex], ref arrayFirst[subIndex], ref arraySecond[subIndex]);
			}
		}
	}

	private int SubIndex(int index)
	{
		return (index >>> 8 * (_level - 1)) & 0xFF;
	}
}
