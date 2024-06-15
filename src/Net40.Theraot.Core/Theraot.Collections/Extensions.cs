using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Theraot.Collections.Specialized;
using Theraot.Threading.Needles;

namespace Theraot.Collections;

[DebuggerNonUserCode]
public static class Extensions
{
    private sealed class NeedleEnumerable<TSource> : IEnumerable<ReadOnlyStructNeedle<TSource>>, IEnumerable
    {
        private readonly IEnumerable<TSource> _source;

        public NeedleEnumerable(IEnumerable<TSource> source)
        {
                _source = source;
            }

        public IEnumerator<ReadOnlyStructNeedle<TSource>> GetEnumerator()
        {
                foreach (TSource item in _source)
                {
                    yield return new ReadOnlyStructNeedle<TSource>(item);
                }
            }

        IEnumerator IEnumerable.GetEnumerator()
        {
                return GetEnumerator();
            }
    }

    public static int AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            int num = 0;
            foreach (T item in items)
            {
                collection.Add(item);
                num++;
            }

            return num;
        }

    public static IEnumerable<ReadOnlyStructNeedle<TSource>> AsNeedleEnumerable<TSource>(
        this IEnumerable<TSource> source)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return new NeedleEnumerable<TSource>(source);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [return: NotNull]
    public static T[] AsArray<T>(this IEnumerable<T>? source)
    {
            if (source == null)
            {
                return ArrayEx.Empty<T>();
            }

            if (source is T[] result)
            {
                return result;
            }

            if (!(source is ICollection<T> collection))
            {
                return new List<T>(source).ToArray();
            }

            if (collection.Count == 0)
            {
                return ArrayEx.Empty<T>();
            }

            T[] array = new T[collection.Count];
            collection.CopyTo(array, 0);
            return array;
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [return: NotNull]
    public static ICollection<T> AsDistinctICollection<T>(this IEnumerable<T>? source)
    {
            return source.AsISet();
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [return: NotNull]
    public static ICollection<T> AsDistinctICollection<T>(this IEnumerable<T>? source,
        IEqualityComparer<T>? comparer)
    {
            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }

            return source.AsISet(comparer);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [return: NotNull]
    public static ICollection<T> AsICollection<T>(this IEnumerable<T>? source)
    {
            if (source == null)
            {
                return ArrayEx.Empty<T>();
            }

            if (source is ICollection<T> result)
            {
                return result;
            }

            return EnumerationList<T>.Create(source);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [return: NotNull]
    public static IList<T> AsIList<T>(this IEnumerable<T>? source)
    {
            if (source == null)
            {
                return ArrayEx.Empty<T>();
            }

            if (source is IList<T> result)
            {
                return result;
            }

            if (!(source is ICollection<T> collection))
            {
                return EnumerationList<T>.Create(source);
            }

            if (collection.Count == 0)
            {
                return ArrayEx.Empty<T>();
            }

            T[] array = new T[collection.Count];
            collection.CopyTo(array, 0);
            return array;
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [return: NotNull]
    public static IReadOnlyCollection<T> AsIReadOnlyCollection<T>(this IEnumerable<T>? source)
    {
            if (source == null)
            {
                return EmptyCollection<T>.Instance;
            }

            if (source is T[] wrapped)
            {
                return new ReadOnlyCollectionEx<T>(wrapped);
            }

            if (source is ListEx<T> listEx)
            {
                return listEx.AsReadOnly();
            }

            if (source is IReadOnlyCollection<T> result)
            {
                return result;
            }

            return EnumerationList<T>.Create(source);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [return: NotNull]
    public static IReadOnlyList<T> AsIReadOnlyList<T>(this IEnumerable<T>? source)
    {
            if (source == null)
            {
                return EmptyCollection<T>.Instance;
            }

            if (source is T[] wrapped)
            {
                return new ReadOnlyCollectionEx<T>(wrapped);
            }

            if (source is ListEx<T> listEx)
            {
                return listEx.AsReadOnly();
            }

            if (source is IReadOnlyList<T> result)
            {
                return result;
            }

            return EnumerationList<T>.Create(source);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [return: NotNull]
    public static ISet<T> AsISet<T>(this IEnumerable<T>? source)
    {
            if (source == null)
            {
                return EmptySet<T>.Instance;
            }

            if (source is ISet<T> result)
            {
                return result;
            }

            return ProgressiveSet<T>.Create(Progressor<T>.CreateFromIEnumerable(source), new HashSetEx<T>(),
                EqualityComparer<T>.Default);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [return: NotNull]
    public static ISet<T> AsISet<T>(this IEnumerable<T>? source, IEqualityComparer<T>? comparer)
    {
            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }

            if (source == null)
            {
                return EmptySet<T>.Instance;
            }

            if (source is HashSet<T> hashSet && hashSet.Comparer.Equals(comparer))
            {
                return hashSet;
            }

            if (source is SortedSet<T> sortedSet && sortedSet.Comparer.Equals(comparer))
            {
                return sortedSet;
            }

            if (source is ProgressiveSet<T> progressiveSet && progressiveSet.Comparer.Equals(comparer))
            {
                return progressiveSet;
            }

            return ProgressiveSet<T>.Create(Progressor<T>.CreateFromIEnumerable(source), new HashSetEx<T>(), comparer);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [return: NotNull]
    public static List<T> AsList<T>(this IEnumerable<T>? source)
    {
            if (source == null)
            {
                return new List<T>();
            }

            if (source is T[] collection)
            {
                return new List<T>(collection);
            }

            if (source is List<T> result)
            {
                return result;
            }

            if (!(source is ICollection<T> collection2))
            {
                return new List<T>(source);
            }

            if (collection2.Count == 0)
            {
                return new List<T>();
            }

            T[] array = new T[collection2.Count];
            collection2.CopyTo(array, 0);
            return new List<T>(array);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [return: NotNull]
    public static IEnumerable<T> AsUnaryIEnumerable<T>(this T source)
    {
            yield return source;
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [return: NotNull]
    public static ReadOnlyCollectionEx<T> ToReadOnlyCollection<T>(this IEnumerable<T>? enumerable)
    {
            if (enumerable == null)
            {
                return EmptyCollection<T>.Instance;
            }

            if (enumerable is ReadOnlyCollectionEx<T> result)
            {
                return result;
            }

            if (enumerable is T[] array)
            {
                return (array.Length == 0) ? EmptyCollection<T>.Instance : new ReadOnlyCollectionEx<T>(array);
            }

            if (!(enumerable is ICollection<T> collection))
            {
                return new ReadOnlyCollectionEx<T>(new List<T>(enumerable));
            }

            if (collection.Count == 0)
            {
                return EmptyCollection<T>.Instance;
            }

            T[] array2 = new T[collection.Count];
            collection.CopyTo(array2, 0);
            return new ReadOnlyCollectionEx<T>(array2);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [return: NotNull]
    public static bool TryGetComparer<TKey, TValue>(this IDictionary<TKey, TValue> source,
        [NotNullWhen(true)] out IEqualityComparer<TKey>? comparer)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (source is IHasComparer<TKey> hasComparer)
            {
                comparer = hasComparer.Comparer;
                return true;
            }

            if (source is Dictionary<TKey, TValue> dictionary)
            {
                comparer = dictionary.Comparer;
                return true;
            }

            comparer = null;
            return false;
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [return: NotNull]
    public static IDictionary<TKey, TValue> WithComparer<TKey, TValue>(this IDictionary<TKey, TValue> source,
        IEqualityComparer<TKey>? comparer)
    {
            if (comparer == null)
            {
                comparer = EqualityComparer<TKey>.Default;
            }

            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (source is IHasComparer<TKey> hasComparer && hasComparer.Comparer.Equals(comparer))
            {
                return source;
            }

            if (source is Dictionary<TKey, TValue> dictionary && dictionary.Comparer.Equals(comparer))
            {
                return dictionary;
            }

            return new DictionaryEx<TKey, TValue>(source, comparer);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [return: NotNull]
    public static ICollection<T> WrapAsICollection<T>(this IEnumerable<T> source)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (source is ICollection<T> result)
            {
                return result;
            }

            return EnumerationList<T>.Create(source);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [return: NotNull]
    public static IList<T> WrapAsIList<T>(this IEnumerable<T> source)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (source is IList<T> result)
            {
                return result;
            }

            return EnumerationList<T>.Create(source);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [return: NotNull]
    public static IReadOnlyCollection<T> WrapAsIReadOnlyCollection<T>(this IEnumerable<T> source)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (source is T[] wrapped)
            {
                return new ReadOnlyCollectionEx<T>(wrapped);
            }

            if (source is ListEx<T> listEx)
            {
                return listEx.AsReadOnly();
            }

            if (source is IReadOnlyCollection<T> result)
            {
                return result;
            }

            return EnumerationList<T>.Create(source);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [return: NotNull]
    public static IReadOnlyList<T> WrapAsIReadOnlyList<T>(this IEnumerable<T> source)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (source is T[] wrapped)
            {
                return new ReadOnlyCollectionEx<T>(wrapped);
            }

            if (source is ListEx<T> listEx)
            {
                return listEx.AsReadOnly();
            }

            if (source is IReadOnlyList<T> result)
            {
                return result;
            }

            return EnumerationList<T>.Create(source);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [return: NotNull]
    public static ICollection<T> WrapAsReadOnlyICollection<T>(this IEnumerable<T> source)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (source is T[] array)
            {
                return ArrayEx.AsReadOnly(array);
            }

            if (source is ListEx<T> listEx)
            {
                return listEx.AsReadOnly();
            }

            return EnumerationList<T>.Create(source);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [return: NotNull]
    internal static T[] AsArrayInternal<T>(this IEnumerable<T>? source)
    {
            if (source == null)
            {
                return ArrayEx.Empty<T>();
            }

            if (source is T[] result)
            {
                return result;
            }

            if (source is ReadOnlyCollectionEx<T> readOnlyCollectionEx)
            {
                return (readOnlyCollectionEx.Wrapped is T[] array) ? array : readOnlyCollectionEx.ToArray();
            }

            if (source is ICollection<T> { Count: 0 })
            {
                return ArrayEx.Empty<T>();
            }

            if (!(source is ICollection<T> collection2))
            {
                return new List<T>(source).ToArray();
            }

            T[] array2 = new T[collection2.Count];
            collection2.CopyTo(array2, 0);
            return array2;
        }

    public static bool HasAtLeast<TSource>(this IEnumerable<TSource> source, int count)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (count == 0)
            {
                return true;
            }

            if (source is ICollection<TSource> collection)
            {
                return collection.Count >= count;
            }

            int num = 0;
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    num = checked(num + 1);
                    if (num == count)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    [return: NotNull]
    public static IEnumerable<T> Skip<T>(this IEnumerable<T> source, Predicate<T>? predicateCount, int skipCount)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return (predicateCount == null)
                ? SkipExtracted(source, skipCount)
                : SkipExtracted(source, predicateCount, skipCount);
        }

    [return: NotNull]
    public static IEnumerable<T> Step<T>(this IEnumerable<T> source, int stepCount)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return StepItemsExtracted();

            IEnumerable<T> StepItemsExtracted()
            {
                int count = 0;
                foreach (T item in source)
                {
                    if (count % stepCount == 0)
                    {
                        count++;
                    }
                    else
                    {
                        yield return item;
                        count++;
                    }
                }
            }
        }

    [return: NotNull]
    public static IEnumerable<T> Take<T>(this IEnumerable<T> source, Predicate<T>? predicateCount, int takeCount)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return (predicateCount == null)
                ? TakeExtracted(source, takeCount)
                : TakeExtracted(source, predicateCount, takeCount);
        }

    [return: NotNull]
    public static T[] ToArray<T>(this IEnumerable<T> source, int count)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (count < 0)
            {
                throw new ArgumentNullException("count");
            }

            if (source is ICollection<T> collection && count >= collection.Count)
            {
                T[] array = new T[collection.Count];
                collection.CopyTo(array, 0);
                return array;
            }

            List<T> list = new List<T>(count);
            foreach (T item in source)
            {
                if (list.Count == count)
                {
                    break;
                }

                list.Add(item);
            }

            return list.ToArray();
        }

    private static IEnumerable<T> SkipExtracted<T>(IEnumerable<T> source, int skipCount)
    {
            int count = 0;
            foreach (T item in source)
            {
                if (count < skipCount)
                {
                    count++;
                }
                else
                {
                    yield return item;
                }
            }
        }

    private static IEnumerable<T> SkipExtracted<T>(IEnumerable<T> source, Predicate<T> predicateCount,
        int skipCount)
    {
            int count = 0;
            foreach (T item in source)
            {
                if (count < skipCount)
                {
                    if (predicateCount(item))
                    {
                        count++;
                    }
                }
                else
                {
                    yield return item;
                }
            }
        }

    private static IEnumerable<T> TakeExtracted<T>(IEnumerable<T> source, int takeCount)
    {
            int count = 0;
            foreach (T item in source)
            {
                if (count == takeCount)
                {
                    break;
                }

                yield return item;
                count++;
            }
        }

    private static IEnumerable<T> TakeExtracted<T>(IEnumerable<T> source, Predicate<T> predicateCount,
        int takeCount)
    {
            int count = 0;
            foreach (T item in source)
            {
                if (count == takeCount)
                {
                    break;
                }

                yield return item;
                if (predicateCount(item))
                {
                    count++;
                }
            }
        }

    public static ReadOnlyCollectionEx<T> AddFirst<T>(this ReadOnlyCollection<T> list, T item)
    {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            T[] array = new T[list.Count + 1];
            array[0] = item;
            list.CopyTo(array, 1);
            return ReadOnlyCollectionEx.Create(array);
        }

    public static T[] AddFirst<T>(this T[] array, T item)
    {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            T[] array2 = new T[array.Length + 1];
            array2[0] = item;
            array.CopyTo(array2, 1);
            return array2;
        }

    public static ReadOnlyCollectionEx<T> AddLast<T>(this ReadOnlyCollection<T> list, T item)
    {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            T[] array = new T[list.Count + 1];
            list.CopyTo(array, 0);
            array[list.Count] = item;
            return ReadOnlyCollectionEx.Create(array);
        }

    public static T[] AddLast<T>(this T[] array, T item)
    {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            T[] array2 = new T[array.Length + 1];
            array.CopyTo(array2, 0);
            array2[array.Length] = item;
            return array2;
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static void CanCopyTo(int count, Array array)
    {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (count > array.Length)
            {
                throw new ArgumentException("The array can not contain the number of elements.", "array");
            }
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static void CanCopyTo(int count, Array array, int arrayIndex)
    {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
            }

            if (count > array.Length - arrayIndex)
            {
                throw new ArgumentException("The array can not contain the number of elements.", "array");
            }
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static void CanCopyTo<T>(int count, T[] array)
    {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (count > array.Length)
            {
                throw new ArgumentException("The array can not contain the number of elements.", "array");
            }
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static void CanCopyTo<T>(int count, T[] array, int arrayIndex)
    {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
            }

            if (count > array.Length - arrayIndex)
            {
                throw new ArgumentException("The array can not contain the number of elements.", "array");
            }
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static void CanCopyTo<T>(T[] array, int arrayIndex, int countLimit)
    {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
            }

            if (countLimit < 0)
            {
                throw new ArgumentOutOfRangeException("countLimit", "Non-negative number is required.");
            }

            if (countLimit > array.Length - arrayIndex)
            {
                throw new ArgumentException("The array can not contain the number of elements.", "array");
            }
        }

    public static void Consume<T>(this IEnumerable<T> source)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            foreach (T item in source)
            {
            }
        }

    public static bool ContainsAny<T>(this IEnumerable<T> source, IEnumerable<T> items)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            IEqualityComparer<T> comparer = EqualityComparer<T>.Default;
            ICollection<T> localCollection = source.AsICollection();
            return items.Any((T item) => localCollection.Contains(item, comparer));
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static void ConvertedCopyTo<TCovered, TUncovered>(this IEnumerable<TCovered> source,
        Func<TCovered, TUncovered> conversion, int sourceIndex, TUncovered[] array)
    {
            source.Skip(sourceIndex).ConvertedCopyTo(conversion, array, 0);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static void ConvertedCopyTo<TCovered, TUncovered>(this IEnumerable<TCovered> source,
        Func<TCovered, TUncovered> conversion, int sourceIndex, TUncovered[] array, int arrayIndex)
    {
            source.Skip(sourceIndex).ConvertedCopyTo(conversion, array, arrayIndex);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static void ConvertedCopyTo<TCovered, TUncovered>(this IEnumerable<TCovered> source,
        Func<TCovered, TUncovered> conversion, int sourceIndex, TUncovered[] array, int arrayIndex, int countLimit)
    {
            source.Skip(sourceIndex).Take(countLimit).ConvertedCopyTo(conversion, array, arrayIndex);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static void ConvertedCopyTo<TCovered, TUncovered>(this IEnumerable<TCovered> source,
        Func<TCovered, TUncovered> conversion, TUncovered[] array)
    {
            source.ConvertedCopyTo(conversion, array, 0);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static void ConvertedCopyTo<TUnderlying, TUncovered>(this IEnumerable<TUnderlying> source,
        Func<TUnderlying, TUncovered> conversion, TUncovered[] array, int arrayIndex)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (conversion == null)
            {
                throw new ArgumentNullException("conversion");
            }

            try
            {
                int num = arrayIndex;
                foreach (TUnderlying item in source)
                {
                    array[num] = conversion(item);
                    num++;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new ArgumentException(ex.Message, "array");
            }
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static void ConvertedCopyTo<TCovered, TUncovered>(this IEnumerable<TCovered> source,
        Func<TCovered, TUncovered> conversion, TUncovered[] array, int arrayIndex, int countLimit)
    {
            source.Take(countLimit).ConvertedCopyTo(conversion, array, arrayIndex);
        }

    public static List<TOutput> ConvertFiltered<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter,
        Predicate<T> filter)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }

            return (from item in source
                where filter(item)
                select converter(item)).ToList();
        }

    public static IEnumerable<TOutput> ConvertProgressive<T, TOutput>(this IEnumerable<T> source,
        Func<T, TOutput> converter)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            return ConvertProgressiveExtracted();

            IEnumerable<TOutput> ConvertProgressiveExtracted()
            {
                foreach (T item in source)
                {
                    yield return converter(item);
                }
            }
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static void CopyTo<T>(this IEnumerable<T> source, int sourceIndex, T[] array)
    {
            source.Skip(sourceIndex).CopyTo(array, 0);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static void CopyTo<T>(this IEnumerable<T> source, int sourceIndex, T[] array, int arrayIndex)
    {
            source.Skip(sourceIndex).CopyTo(array, arrayIndex);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static void CopyTo<T>(this IEnumerable<T> source, int sourceIndex, T[] array, int arrayIndex,
        int countLimit)
    {
            source.Skip(sourceIndex).Take(countLimit).CopyTo(array, arrayIndex);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static void CopyTo<T>(this IEnumerable<T> source, T[] array)
    {
            source.CopyTo(array, 0);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static void CopyTo<T>(this IEnumerable<T> source, T[] array, int arrayIndex)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            try
            {
                int num = arrayIndex;
                foreach (T item in source)
                {
                    array[num] = item;
                    num++;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new ArgumentException(ex.Message, "array");
            }
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static void CopyTo<T>(this IEnumerable<T> source, T[] array, int arrayIndex, int countLimit)
    {
            source.Take(countLimit).CopyTo(array, arrayIndex);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static void DeprecatedCopyTo<T>(this IEnumerable<T> source, Array array)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            int num = 0;
            foreach (T item in source)
            {
                array.SetValue(item, num++);
            }
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static void DeprecatedCopyTo<T>(this IEnumerable<T> source, Array array, int index)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            foreach (T item in source)
            {
                array.SetValue(item, index++);
            }
        }

    public static bool Dequeue<T>(this Queue<T> source, T item, IEqualityComparer<T> comparer)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }

            if (!comparer.Equals(item, source.Peek()))
            {
                return false;
            }

            source.Dequeue();
            return true;
        }

    public static int ExceptWith<T>(this ICollection<T> source, IEnumerable<T> other)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            int num = 0;
            foreach (T item in other)
            {
                while (source.Remove(item))
                {
                    num++;
                }
            }

            return num;
        }

    public static IEnumerable<T> ExceptWithEnumerable<T>(this ICollection<T> source, IEnumerable<T> other)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            return ExceptWithEnumerableExtracted();

            IEnumerable<T> ExceptWithEnumerableExtracted()
            {
                foreach (T item in other)
                {
                    while (source.Remove(item))
                    {
                        yield return item;
                    }
                }
            }
        }

    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return FlattenExtracted();

            IEnumerable<T> FlattenExtracted()
            {
                foreach (IEnumerable<T> key in source)
                {
                    foreach (T item in key)
                    {
                        yield return item;
                    }
                }
            }
        }

    public static int IndexOf<T>(this IEnumerable<T> source, T item)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            int num = 0;
            EqualityComparer<T> @default = EqualityComparer<T>.Default;
            foreach (T item2 in source)
            {
                if (@default.Equals(item2, item))
                {
                    return num;
                }

                num++;
            }

            return -1;
        }

    public static int IndexOf<T>(this IEnumerable<T> source, T item, IEqualityComparer<T> comparer)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            int currentIndex = 0;
            return (comparer == null)
                ? IndexOfExtracted(source, item, EqualityComparer<T>.Default, ref currentIndex)
                : IndexOfExtracted(source, item, comparer, ref currentIndex);
        }

    public static int IntersectWith<T>(this ICollection<T> source, IEnumerable<T> other)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            ICollection<T> otherAsCollection = other.AsICollection();
            return source.RemoveWhere((T input) => !otherAsCollection.Contains(input));
        }

    public static int IntersectWith<T>(this ICollection<T> source, IEnumerable<T> other,
        IEqualityComparer<T> comparer)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }

            ICollection<T> otherAsCollection = other.AsICollection();
            return source.RemoveWhere((T input) => !otherAsCollection.Contains(input, comparer));
        }

    public static bool IsProperSubsetOf<T>(this IEnumerable<T> source, IEnumerable<T> other)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            return source.IsSubsetOf(other, proper: true);
        }

    public static bool IsProperSupersetOf<T>(this IEnumerable<T> source, IEnumerable<T> other)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            return source.IsSupersetOf(other, proper: true);
        }

    public static bool IsSubsetOf<T>(this IEnumerable<T> source, IEnumerable<T> other)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            return source.IsSubsetOf(other, proper: false);
        }

    public static bool IsSupersetOf<T>(this IEnumerable<T> source, IEnumerable<T> other)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            return source.IsSupersetOf(other, proper: false);
        }

    public static bool ListEquals<T>(this IList<T> first, IList<T> second)
    {
            if (first == second)
            {
                return true;
            }

            if (first == null || second == null)
            {
                return false;
            }

            int count = first.Count;
            if (count != second.Count)
            {
                return false;
            }

            EqualityComparer<T> @default = EqualityComparer<T>.Default;
            for (int i = 0; i != count; i++)
            {
                if (!@default.Equals(first[i], second[i]))
                {
                    return false;
                }
            }

            return true;
        }

    public static void Move<T>(this IList<T> list, int oldIndex, int newIndex)
    {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            T item = list[oldIndex];
            list.RemoveAt(oldIndex);
            if (newIndex > oldIndex)
            {
                newIndex--;
            }

            list.Insert(newIndex, item);
        }

    public static bool Overlaps<T>(this IEnumerable<T> source, IEnumerable<T> items)
    {
            return source.ContainsAny(items);
        }

    public static bool Pop<T>(this Stack<T> source, T item, IEqualityComparer<T> comparer)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }

            if (!comparer.Equals(item, source.Peek()))
            {
                return false;
            }

            source.Pop();
            return true;
        }

    public static bool ReadOnlyListEquals<T>(this IReadOnlyList<T> first, IReadOnlyList<T> second)
    {
            if (first == second)
            {
                return true;
            }

            if (first == null || second == null)
            {
                return false;
            }

            int count = first.Count;
            if (count != second.Count)
            {
                return false;
            }

            EqualityComparer<T> @default = EqualityComparer<T>.Default;
            for (int i = 0; i != count; i++)
            {
                if (!@default.Equals(first[i], second[i]))
                {
                    return false;
                }
            }

            return true;
        }

    public static bool Remove<T>(this ICollection<T> source, T item, IEqualityComparer<T> comparer)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }

            return source.RemoveWhereEnumerable((T input) => comparer.Equals(input, item)).Any();
        }

    public static T[] RemoveFirst<T>(this T[] array)
    {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            T[] array2 = new T[array.Length - 1];
            Array.Copy(array, 1, array2, 0, array2.Length);
            return array2;
        }

    public static T[] RemoveLast<T>(this T[] array)
    {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            T[] array2 = new T[array.Length - 1];
            Array.Copy(array, 0, array2, 0, array2.Length);
            return array2;
        }

    public static int RemoveWhere<T>(this ICollection<T> source, Func<IEnumerable<T>, IEnumerable<T>> converter)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            return source.ExceptWith(new List<T>(converter(source)));
        }

    public static int RemoveWhere<T>(this ICollection<T> source, Func<T, bool> predicate)
    {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            return source.RemoveWhere((IEnumerable<T> items) => items.Where(predicate));
        }

    public static IEnumerable<T> RemoveWhereEnumerable<T>(this ICollection<T> source,
        Func<IEnumerable<T>, IEnumerable<T>> converter)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            return source.ExceptWithEnumerable(new List<T>(converter(source)));
        }

    public static IEnumerable<T> RemoveWhereEnumerable<T>(this ICollection<T> source, Func<T, bool> predicate)
    {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            return source.RemoveWhereEnumerable((IEnumerable<T> items) => items.Where(predicate));
        }

    public static bool SetEquals<T>(this ICollection<T> source, IEnumerable<T> other)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            ICollection<T> thatAsCollection = other.AsICollection();
            return thatAsCollection.All(source.Contains) && source.All((T input) => thatAsCollection.Contains(input));
        }

    public static void Swap<T>(this IList<T> list, int indexA, int indexB)
    {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            if (indexA < 0)
            {
                throw new ArgumentOutOfRangeException("indexA", "Non-negative number is required.");
            }

            if (indexB < 0)
            {
                throw new ArgumentOutOfRangeException("indexB", "Non-negative number is required.");
            }

            int count = list.Count;
            if (indexA >= count || indexB >= count)
            {
                throw new ArgumentException("The list does not contain the number of elements.", "list");
            }

            if (indexA != indexB)
            {
                SwapExtracted(list, indexA, indexB);
            }
        }

    public static int SymmetricExceptWith<T>(this ICollection<T> source, IEnumerable<T> other)
    {
            return source.AddRange(from input in other.Distinct()
                where !source.Remove(input)
                select input);
        }

    public static bool TryTake<T>(this Stack<T> stack, [MaybeNullWhen(false)] out T item)
    {
            if (stack == null)
            {
                throw new ArgumentNullException("stack");
            }

            try
            {
                item = stack.Pop();
                return true;
            }
            catch (InvalidOperationException)
            {
                item = default(T);
                return false;
            }
        }

    public static int UnionWith<T>(this ICollection<T> source, IEnumerable<T> other)
    {
            return source.AddRange(other.Where((T input) => !source.Contains(input)));
        }

    private static int IndexOfExtracted<T>(IEnumerable<T> source, T item, IEqualityComparer<T> comparer,
        ref int currentIndex)
    {
            foreach (T item2 in source)
            {
                if (comparer.Equals(item2, item))
                {
                    return currentIndex;
                }

                currentIndex++;
            }

            return -1;
        }

    private static bool IsSubsetOf<T>(this IEnumerable<T> source, IEnumerable<T> other, bool proper)
    {
            ICollection<T> collection = source.AsDistinctICollection();
            ICollection<T> collection2 = other.AsDistinctICollection();
            int num = 0;
            int num2 = 0;
            foreach (T item in collection2)
            {
                num++;
                if (collection.Contains(item))
                {
                    num2++;
                }
            }

            if (proper)
            {
                return num2 == collection.Count && num > collection.Count;
            }

            return num2 == collection.Count;
        }

    private static bool IsSupersetOf<T>(this IEnumerable<T> source, IEnumerable<T> other, bool proper)
    {
            ICollection<T> collection = source.AsDistinctICollection();
            ICollection<T> collection2 = other.AsDistinctICollection();
            int num = 0;
            foreach (T item in collection2)
            {
                num++;
                if (!collection.Contains(item))
                {
                    return false;
                }
            }

            if (proper)
            {
                return num < collection.Count;
            }

            return true;
        }

    private static void SwapExtracted<T>(IList<T> list, int indexA, int indexB)
    {
            T value = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = value;
        }

    public static List<TOutput> ConvertAll<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            return source.Select(converter).ToList();
        }

    /*public static TList ConvertAll<T, TOutput, TList>(this IEnumerable<T> source, Func<T, TOutput> converter) where TList : ICollection<TOutput>, new()
    {
        if (source == null)
        {
            throw new ArgumentNullException("source");
        }
        if (converter == null)
        {
            throw new ArgumentNullException("converter");
        }
        TList result = new TList();
        foreach (T item in source)
        {
            ref TList reference = ref result;
            TList val = default(TList);
            if (val == null)
            {
                val = reference;
                reference = ref val;
            }
            reference.Add(converter(item));
        }
        return result;
    }*/

    public static TList ConvertAll<T, TOutput, TList>(this IEnumerable<T> source, Func<T, TOutput> converter)
        where TList : ICollection<TOutput>, new()
    {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            var result = new TList();
            foreach (var item in source)
            {
                result.Add(converter(item));
            }

            return result;
        }

    public static int CountContiguousItems<T>(this IEnumerable<T> source, T item)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            int num = 0;
            EqualityComparer<T> @default = EqualityComparer<T>.Default;
            foreach (T item2 in source)
            {
                if (@default.Equals(item2, item))
                {
                    num++;
                    continue;
                }

                break;
            }

            return num;
        }

    public static int CountContiguousItemsWhere<T>(this IEnumerable<T> source, Predicate<T> predicate)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            int num = 0;
            foreach (T item in source)
            {
                if (predicate(item))
                {
                    num++;
                    continue;
                }

                break;
            }

            return num;
        }

    public static int CountItems<T>(this IEnumerable<T> source, T item)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
            return source.Count((T value) => equalityComparer.Equals(value, item));
        }

    public static int CountItemsWhere<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            return source.Count(predicate);
        }

    public static bool TryDequeue<T>(this Queue<T> source, [MaybeNullWhen(false)] out T item)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            try
            {
                item = source.Dequeue();
                return true;
            }
            catch (InvalidOperationException)
            {
                item = default(T);
                return false;
            }
        }

    public static bool TryPeek<T>(this Stack<T> source, [MaybeNullWhen(false)] out T item)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            try
            {
                item = source.Peek();
                return true;
            }
            catch (InvalidOperationException)
            {
                item = default(T);
                return false;
            }
        }

    public static bool TryPeek<T>(this Queue<T> source, [MaybeNullWhen(false)] out T item)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            try
            {
                item = source.Peek();
                return true;
            }
            catch (InvalidOperationException)
            {
                item = default(T);
                return false;
            }
        }

    public static bool TryPop<T>(this Stack<T> source, [MaybeNullWhen(false)] out T item)
    {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            try
            {
                item = source.Pop();
                return true;
            }
            catch (InvalidOperationException)
            {
                item = default(T);
                return false;
            }
        }

    public static int ComputeHashCode<T>(this IEnumerable<T> collection)
    {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            EqualityComparer<T> @default = EqualityComparer<T>.Default;
            int num = 6551;
            foreach (T item in collection)
            {
                int num2;
                try
                {
                    num2 = @default.GetHashCode(item);
                }
                catch (ArgumentNullException)
                {
                    num2 = 0;
                }

                num ^= (num << 5) ^ num2;
            }

            return num;
        }
}