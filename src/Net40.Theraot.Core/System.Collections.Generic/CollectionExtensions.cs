using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic;

public static class CollectionExtensions
{
    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [return: MaybeNull]
    public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
    {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            return dictionary.TryGetValue(key, out var value) ? value : default;
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
    {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static bool Remove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, [MaybeNullWhen(false)] out TValue value)
    {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (dictionary.TryGetValue(key, out value) && dictionary.Remove(key))
            {
                return true;
            }

            value = default;
            return false;
        }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            try
            {
                dictionary.Add(key, value);
                return true;
            }
            catch (ArgumentException ex)
            {
                _ = ex;
                return false;
            }
        }
}