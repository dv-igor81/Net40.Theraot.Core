using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Threading;
using Theraot.Threading;

namespace Theraot.Collections.ThreadSafe;

[Serializable]
public sealed class FixedSizeBucket<T> : IBucket<T>, IEnumerable<T>, IEnumerable, ISerializable
{
    private readonly object[] _entries;

    private int _count;

    public int Capacity { get; }

    public int Count => _count;

    public FixedSizeBucket(int capacity)
    {
            _count = 0;
            _entries = ArrayReservoir<object>.GetArray(capacity);
            Capacity = _entries.Length;
        }

    public FixedSizeBucket(IEnumerable<T> source)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            _entries = ArrayReservoir<object>.GetArray((source as ICollection<T>)?.Count ?? 64);
            Capacity = _entries.Length;
            foreach (T item in source)
            {
                if (_count == Capacity)
                {
                    object[] entries = _entries;
                    _entries = ArrayReservoir<object>.GetArray(Capacity << 1);
                    Array.Copy(entries, 0, _entries, 0, _count);
                    ArrayReservoir<object>.DonateArray(entries);
                    Capacity = _entries.Length;
                }

                object[] entries2 = _entries;
                int count = _count;
                T val = item;
                entries2[count] = ((val != null) ? ((object)val) : BucketHelper.Null);
                _count++;
            }
        }

    private FixedSizeBucket(SerializationInfo info, StreamingContext context)
    {
            if (!(info.GetValue("count", typeof(int)) is int val) ||
                !(info.GetValue("contents", typeof(object[])) is object[] array))
            {
                throw new SerializationException();
            }

            _count = Math.Min(val, array.Length);
            _entries = array;
            Capacity = _entries.Length;
        }

    ~FixedSizeBucket()
    {
            if (!GCMonitor.FinalizingForUnload)
            {
                object[] entries = _entries;
                if (entries != null)
                {
                    ArrayReservoir<object>.DonateArray(entries);
                }
            }
        }

    public void CopyTo(T[] array, int arrayIndex)
    {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
            }

            if (_count > array.Length - arrayIndex)
            {
                throw new ArgumentException("The array can not contain the number of elements.", "array");
            }

            try
            {
                object[] entries = _entries;
                foreach (object obj in entries)
                {
                    if (obj != null)
                    {
                        if (obj == BucketHelper.Null)
                        {
                            array[arrayIndex] = default(T);
                        }
                        else
                        {
                            array[arrayIndex] = (T)obj;
                        }

                        arrayIndex++;
                    }
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new ArgumentOutOfRangeException("array", ex.Message);
            }
        }

    public bool Exchange(int index, T item, out T previous)
    {
            if (index < 0 || index >= Capacity)
            {
                throw new ArgumentOutOfRangeException("index",
                    "index must be greater or equal to 0 and less than capacity");
            }

            return ExchangeInternal(index, item, out previous);
        }

    public IEnumerator<T> GetEnumerator()
    {
            object[] entries = _entries;
            foreach (object entry in entries)
            {
                if (entry != null)
                {
                    if (entry == BucketHelper.Null)
                    {
                        yield return default(T);
                    }
                    else
                    {
                        yield return (T)entry;
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
            info.AddValue("count", _count, typeof(int));
            info.AddValue("contents", _entries, typeof(object[]));
        }

    public bool Insert(int index, T item)
    {
            if (index < 0 || index >= Capacity)
            {
                throw new ArgumentOutOfRangeException("index",
                    "index must be greater or equal to 0 and less than capacity.");
            }

            return InsertInternal(index, item);
        }

    public bool Insert(int index, T item, out T previous)
    {
            if (index < 0 || index >= Capacity)
            {
                throw new ArgumentOutOfRangeException("index",
                    "index must be greater or equal to 0 and less than capacity");
            }

            return InsertInternal(index, item, out previous);
        }

    public bool RemoveAt(int index)
    {
            if (index < 0 || index >= Capacity)
            {
                throw new ArgumentOutOfRangeException("index",
                    "index must be greater or equal to 0 and less than capacity");
            }

            object obj = Interlocked.Exchange(ref _entries[index], null);
            if (obj == null)
            {
                return false;
            }

            Interlocked.Decrement(ref _count);
            return true;
        }

    public bool RemoveAt(int index, out T previous)
    {
            if (index < 0 || index >= Capacity)
            {
                throw new ArgumentOutOfRangeException("index",
                    "index must be greater or equal to 0 and less than capacity");
            }

            return RemoveAtInternal(index, out previous);
        }

    public bool RemoveAt(int index, Predicate<T> check)
    {
            if (index < 0 || index >= Capacity)
            {
                throw new ArgumentOutOfRangeException("index",
                    "index must be greater or equal to 0 and less than capacity");
            }

            if (check == null)
            {
                throw new ArgumentNullException("check");
            }

            object obj = Interlocked.CompareExchange(ref _entries[index], null, null);
            if (obj == null)
            {
                return false;
            }

            T obj2 = ((obj == BucketHelper.Null) ? default(T) : ((T)obj));
            if (!check(obj2))
            {
                return false;
            }

            object obj3 = Interlocked.CompareExchange(ref _entries[index], null, obj);
            if (obj != obj3)
            {
                return false;
            }

            Interlocked.Decrement(ref _count);
            return true;
        }

    public void Set(int index, T item, out bool isNew)
    {
            if (index < 0 || index >= Capacity)
            {
                throw new ArgumentOutOfRangeException("index",
                    "index must be greater or equal to 0 and less than capacity");
            }

            SetInternal(index, item, out isNew);
        }

    public bool TryGet(int index, out T value)
    {
            if (index < 0 || index >= Capacity)
            {
                throw new ArgumentOutOfRangeException("index",
                    "index must be greater or equal to 0 and less than capacity");
            }

            return TryGetInternal(index, out value);
        }

    public bool Update(int index, Func<T, T> itemUpdateFactory, Predicate<T> check, out bool isEmpty)
    {
            if (index < 0 || index >= Capacity)
            {
                throw new ArgumentOutOfRangeException("index",
                    "index must be greater or equal to 0 and less than capacity.");
            }

            if (itemUpdateFactory == null)
            {
                throw new ArgumentNullException("itemUpdateFactory");
            }

            if (check == null)
            {
                throw new ArgumentNullException("check");
            }

            return UpdateInternal(index, itemUpdateFactory, check, out isEmpty);
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
                object[] entries = _entries;
                foreach (object entry in entries)
                {
                    if (entry != null)
                    {
                        T castValue = ((entry == BucketHelper.Null) ? default(T) : ((T)entry));
                        if (check(castValue))
                        {
                            yield return castValue;
                        }
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
                object[] entries = _entries;
                foreach (object entry in entries)
                {
                    if (entry != null)
                    {
                        T castValue = ((entry == BucketHelper.Null) ? default(T) : ((T)entry));
                        if (check(castValue))
                        {
                            yield return new KeyValuePair<int, T>(index, castValue);
                            index++;
                        }
                    }
                }
            }
        }

    internal bool ExchangeInternal(int index, T item, out T previous)
    {
            previous = default(T);
            object obj = Interlocked.Exchange(ref _entries[index], (item != null) ? ((object)item) : BucketHelper.Null);
            if (obj == null)
            {
                Interlocked.Increment(ref _count);
                return true;
            }

            if (obj != BucketHelper.Null)
            {
                previous = (T)obj;
            }

            return false;
        }

    internal bool InsertInternal(int index, T item, out T previous)
    {
            previous = default(T);
            object obj = Interlocked.CompareExchange(ref _entries[index],
                (item != null) ? ((object)item) : BucketHelper.Null, null);
            if (obj == null)
            {
                Interlocked.Increment(ref _count);
                return true;
            }

            if (obj != BucketHelper.Null)
            {
                previous = (T)obj;
            }

            return false;
        }

    internal bool InsertInternal(int index, T item)
    {
            object[] entries = _entries;
            if (entries == null)
            {
                return false;
            }

            object obj = Interlocked.CompareExchange(ref entries[index],
                (item != null) ? ((object)item) : BucketHelper.Null, null);
            if (obj != null)
            {
                return false;
            }

            Interlocked.Increment(ref _count);
            return true;
        }

    internal bool RemoveAtInternal(int index, [NotNullWhen(true)] out T previous)
    {
            previous = default(T);
            object obj = Interlocked.Exchange(ref _entries[index], null);
            if (obj == null)
            {
                return false;
            }

            Interlocked.Decrement(ref _count);
            if (obj != BucketHelper.Null)
            {
                previous = (T)obj;
            }

            return true;
        }

    internal void SetInternal(int index, T item, out bool isNew)
    {
            isNew = Interlocked.Exchange(ref _entries[index], (item != null) ? ((object)item) : BucketHelper.Null) ==
                    null;
            if (isNew)
            {
                Interlocked.Increment(ref _count);
            }
        }

    internal bool TryGetInternal(int index, [NotNullWhen(true)] out T value)
    {
            object obj = Interlocked.CompareExchange(ref _entries[index], null, null);
            if (obj == null)
            {
                value = default(T);
                return false;
            }

            value = ((obj == BucketHelper.Null) ? default(T) : ((T)obj));
            return true;
        }

    internal bool UpdateInternal(int index, Func<T, T> itemUpdateFactory, Predicate<T> check, out bool isEmpty)
    {
            object obj = Interlocked.CompareExchange(ref _entries[index], null, null);
            object obj2 = BucketHelper.Null;
            bool result = false;
            if (obj != null)
            {
                T val = ((obj == BucketHelper.Null) ? default(T) : ((T)obj));
                if (check(val))
                {
                    T val2 = itemUpdateFactory(val);
                    ref object location = ref _entries[index];
                    T val3 = val2;
                    obj2 = Interlocked.CompareExchange(ref location,
                        (val3 != null) ? ((object)val3) : BucketHelper.Null, obj);
                    result = obj == obj2;
                }
            }

            isEmpty = obj == null || obj2 == null;
            return result;
        }
}