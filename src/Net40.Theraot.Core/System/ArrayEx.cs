using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Theraot.Collections.ThreadSafe;

// ReSharper disable once CheckNamespace
namespace System;

public static class ArrayEx
{
    private static readonly CacheDict<Type, Array> _emptyArrays = new CacheDict<Type, Array>(256);

    private const int _maxArrayLength = 2146435071;

    private const int _maxByteArrayLength = 2147483591;

    public static int MaxArrayLength => 2146435071;

    public static int MaxByteArrayLength => 2147483591;

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static T[] Empty<T>()
    {
            Type typeFromHandle = typeof(T);
            if (typeFromHandle == typeof(Type))
            {
                return (T[])(object)TypeEx.EmptyTypes;
            }

            if (_emptyArrays.TryGetValue(typeFromHandle, out var value))
            {
                return (T[])value;
            }

            T[] array = new T[0];
            _emptyArrays[typeFromHandle] = array;
            return array;
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [return: NotNull]
    public static ReadOnlyCollection<T> AsReadOnly<T>(T[] array)
    {
            return Array.AsReadOnly(array);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [return: NotNull]
    public static TOutput[] ConvertAll<TInput, TOutput>(TInput[] array, Converter<TInput, TOutput> converter)
    {
            return Array.ConvertAll(array, converter);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static void Copy(Array sourceArray, Array destinationArray, long length)
    {
            Array.Copy(sourceArray, destinationArray, length);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static void Copy(Array sourceArray, long sourceIndex, Array destinationArray, long destinationIndex,
        long length)
    {
            Array.Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, length);
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static void Fill<T>(T[] array, T value)
    {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static void ForEach<T>(T[] array, Action<T> action)
    {
            Array.ForEach(array, action);
        }
}