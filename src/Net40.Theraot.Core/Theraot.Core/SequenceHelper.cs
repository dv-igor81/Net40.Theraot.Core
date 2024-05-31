using System;
using System.Collections.Generic;
using System.Linq;

namespace Theraot.Core;

public static class SequenceHelper
{
	public static IEnumerable<T> ExploreSequence<T>(T initial, T endCondition, Func<T, T> next)
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		return ExploreSequenceExtracted(initial, endCondition, next, FuncHelper.GetIdentityFunc<T>(), null);
	}

	public static IEnumerable<T> ExploreSequence<T>(T initial, T endCondition, Func<T, T> next, IEqualityComparer<T>? comparer)
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		return ExploreSequenceExtracted(initial, endCondition, next, FuncHelper.GetIdentityFunc<T>(), comparer);
	}

	public static IEnumerable<TOutput> ExploreSequence<TInput, TOutput>(TInput initial, TInput endCondition, Func<TInput, TInput> next, Func<TInput, TOutput> resultSelector)
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		if (resultSelector == null)
		{
			throw new ArgumentNullException("resultSelector");
		}
		return ExploreSequenceExtracted(initial, endCondition, next, resultSelector, EqualityComparer<TInput>.Default);
	}

	public static IEnumerable<TOutput> ExploreSequence<TInput, TOutput>(TInput initial, TInput endCondition, Func<TInput, TInput> next, Func<TInput, TOutput> resultSelector, IEqualityComparer<TInput>? comparer)
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		if (resultSelector == null)
		{
			throw new ArgumentNullException("resultSelector");
		}
		return ExploreSequenceExtracted(initial, endCondition, next, resultSelector, comparer);
	}

	private static IEnumerable<TOutput> ExploreSequenceExtracted<TInput, TOutput>(TInput initial, TInput endCondition, Func<TInput, TInput> next, Func<TInput, TOutput> resultSelector, IEqualityComparer<TInput>? comparer)
	{
		if (comparer == null)
		{
			comparer = EqualityComparer<TInput>.Default;
		}
		TInput current = initial;
		while (true)
		{
			yield return resultSelector(current);
			TInput found = next(current);
			if (comparer.Equals(found, endCondition))
			{
				break;
			}
			current = found;
		}
	}

	public static IEnumerable<T> ExploreSequenceUntilNull<T>(T? initial, Func<T, T?> next) where T : class
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		return ExploreSequenceExtractedUntilNull(initial, next, FuncHelper.GetIdentityFunc<T>());
	}

	public static IEnumerable<T> ExploreSequenceUntilNull<T>(T? initial, Func<T, T?> next) where T : struct
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		return ExploreSequenceExtractedUntilNull(initial, next, FuncHelper.GetIdentityFunc<T>());
	}

	public static IEnumerable<TOutput> ExploreSequenceUntilNull<TInput, TOutput>(TInput? initial, Func<TInput, TInput?> next, Func<TInput, TOutput> resultSelector) where TInput : class
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		if (resultSelector == null)
		{
			throw new ArgumentNullException("resultSelector");
		}
		return ExploreSequenceExtractedUntilNull(initial, next, resultSelector);
	}

	public static IEnumerable<TOutput> ExploreSequenceUntilNull<TInput, TOutput>(TInput? initial, Func<TInput, TInput?> next, Func<TInput, TOutput> resultSelector) where TInput : struct
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		if (resultSelector == null)
		{
			throw new ArgumentNullException("resultSelector");
		}
		return ExploreSequenceExtractedUntilNull(initial, next, resultSelector);
	}

	private static IEnumerable<TOutput> ExploreSequenceExtractedUntilNull<TInput, TOutput>(TInput? initial, Func<TInput, TInput?> next, Func<TInput, TOutput> resultSelector) where TInput : class
	{
		TInput current = initial;
		if (current == null)
		{
			yield break;
		}
		while (true)
		{
			yield return resultSelector(current);
			TInput found = next(current);
			if (found == null)
			{
				break;
			}
			current = found;
		}
	}

	private static IEnumerable<TOutput> ExploreSequenceExtractedUntilNull<TInput, TOutput>(TInput? initial, Func<TInput, TInput?> next, Func<TInput, TOutput> resultSelector) where TInput : struct
	{
		TInput? current = initial;
		if (!current.HasValue)
		{
			yield break;
		}
		while (true)
		{
			yield return resultSelector(current.Value);
			TInput? found = next(current.Value);
			if (!found.HasValue)
			{
				break;
			}
			current = found.Value;
		}
	}

	public static IEnumerable<T> ExploreSequenceUntilNull<T>(T? initial, T? endCondition, Func<T, T?> next) where T : class
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		return ExploreSequenceExtractedUntilNull(initial, endCondition, next, FuncHelper.GetIdentityFunc<T>(), EqualityComparer<T>.Default);
	}

	public static IEnumerable<T> ExploreSequenceUntilNull<T>(T? initial, T? endCondition, Func<T, T?> next) where T : struct
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		return ExploreSequenceExtractedUntilNull(initial, endCondition, next, FuncHelper.GetIdentityFunc<T>(), EqualityComparer<T>.Default);
	}

	public static IEnumerable<T> ExploreSequenceUntilNull<T>(T? initial, T? endCondition, Func<T, T?> next, IEqualityComparer<T>? comparer) where T : class
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		if (comparer == null)
		{
			comparer = EqualityComparer<T>.Default;
		}
		return ExploreSequenceExtractedUntilNull(initial, endCondition, next, FuncHelper.GetIdentityFunc<T>(), comparer);
	}

	public static IEnumerable<T> ExploreSequenceUntilNull<T>(T? initial, T? endCondition, Func<T, T?> next, IEqualityComparer<T>? comparer) where T : struct
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		if (comparer == null)
		{
			comparer = EqualityComparer<T>.Default;
		}
		return ExploreSequenceExtractedUntilNull(initial, endCondition, next, FuncHelper.GetIdentityFunc<T>(), comparer);
	}

	public static IEnumerable<TOutput> ExploreSequenceUntilNull<TInput, TOutput>(TInput? initial, TInput endCondition, Func<TInput, TInput?> next, Func<TInput, TOutput> resultSelector) where TInput : class
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		if (resultSelector == null)
		{
			throw new ArgumentNullException("resultSelector");
		}
		return ExploreSequenceExtractedUntilNull(initial, endCondition, next, resultSelector, EqualityComparer<TInput>.Default);
	}

	public static IEnumerable<TOutput> ExploreSequenceUntilNull<TInput, TOutput>(TInput? initial, TInput endCondition, Func<TInput, TInput?> next, Func<TInput, TOutput> resultSelector) where TInput : struct
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		if (resultSelector == null)
		{
			throw new ArgumentNullException("resultSelector");
		}
		return ExploreSequenceExtractedUntilNull(initial, endCondition, next, resultSelector, EqualityComparer<TInput>.Default);
	}

	public static IEnumerable<TOutput> ExploreSequenceUntilNull<TInput, TOutput>(TInput? initial, TInput endCondition, Func<TInput, TInput?> next, Func<TInput, TOutput> resultSelector, IEqualityComparer<TInput>? comparer) where TInput : class
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		if (resultSelector == null)
		{
			throw new ArgumentNullException("resultSelector");
		}
		if (comparer == null)
		{
			comparer = EqualityComparer<TInput>.Default;
		}
		return ExploreSequenceExtractedUntilNull(initial, endCondition, next, resultSelector, comparer);
	}

	public static IEnumerable<TOutput> ExploreSequenceUntilNull<TInput, TOutput>(TInput? initial, TInput endCondition, Func<TInput, TInput?> next, Func<TInput, TOutput> resultSelector, IEqualityComparer<TInput>? comparer) where TInput : struct
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		if (resultSelector == null)
		{
			throw new ArgumentNullException("resultSelector");
		}
		if (comparer == null)
		{
			comparer = EqualityComparer<TInput>.Default;
		}
		return ExploreSequenceExtractedUntilNull(initial, endCondition, next, resultSelector, comparer);
	}

	private static IEnumerable<TOutput> ExploreSequenceExtractedUntilNull<TInput, TOutput>(TInput? initial, TInput? endCondition, Func<TInput, TInput?> next, Func<TInput, TOutput> resultSelector, IEqualityComparer<TInput> comparer) where TInput : class
	{
		TInput current = initial;
		if (current == null)
		{
			yield break;
		}
		while (true)
		{
			yield return resultSelector(current);
			TInput found = next(current);
			if (found == null || (endCondition != null && comparer.Equals(found, endCondition)))
			{
				break;
			}
			current = found;
		}
	}

	private static IEnumerable<TOutput> ExploreSequenceExtractedUntilNull<TInput, TOutput>(TInput? initial, TInput? endCondition, Func<TInput, TInput?> next, Func<TInput, TOutput> resultSelector, IEqualityComparer<TInput> comparer) where TInput : struct
	{
		TInput? current = initial;
		if (!current.HasValue)
		{
			yield break;
		}
		while (true)
		{
			yield return resultSelector(current.Value);
			TInput? found = next(current.Value);
			if (!found.HasValue || (endCondition.HasValue && comparer.Equals(found.Value, endCondition.Value)))
			{
				break;
			}
			current = found.Value;
		}
	}

	public static IEnumerable<T> ExploreCircularSequence<T>(T initial, Func<T, T> next, IEqualityComparer<T>? comparer)
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		if (comparer == null)
		{
			comparer = EqualityComparer<T>.Default;
		}
		return ExploreCircularSequenceExtracted(initial, next, FuncHelper.GetIdentityFunc<T>(), comparer);
	}

	public static IEnumerable<TOutput> ExploreCircularSequence<TInput, TOutput>(TInput initial, Func<TInput, TInput> next, Func<TInput, TOutput> resultSelector, IEqualityComparer<TInput>? comparer)
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		if (resultSelector == null)
		{
			throw new ArgumentNullException("resultSelector");
		}
		if (comparer == null)
		{
			comparer = EqualityComparer<TInput>.Default;
		}
		return ExploreCircularSequenceExtracted(initial, next, resultSelector, comparer);
	}

	private static IEnumerable<TOutput> ExploreCircularSequenceExtracted<TInput, TOutput>(TInput initial, Func<TInput, TInput> next, Func<TInput, TOutput> resultSelector, IEqualityComparer<TInput> comparer)
	{
		HashSet<TInput> known = new HashSet<TInput>(comparer);
		TInput current = initial;
		while (true)
		{
			yield return resultSelector(current);
			known.Add(current);
			TInput found = next(current);
			if (known.Contains(found))
			{
				break;
			}
			current = found;
		}
	}

	public static HashSet<T> GetCircularSet<T>(T initial, Func<T, T> next, IEqualityComparer<T>? comparer)
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		return GetCircularSequenceExtracted(initial, next, comparer ?? EqualityComparer<T>.Default);
	}

	private static HashSet<T> GetCircularSequenceExtracted<T>(T initial, Func<T, T> next, IEqualityComparer<T> comparer)
	{
		HashSet<T> hashSet = new HashSet<T>(comparer);
		T val = initial;
		while (true)
		{
			hashSet.Add(val);
			T val2 = next(val);
			if (hashSet.Contains(val2))
			{
				break;
			}
			val = val2;
		}
		return hashSet;
	}

	public static T? CommonNode<T>(T first, T second, Func<T, T?> next) where T : class
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		return CommonNodeExtracted(first, second, next, EqualityComparer<T>.Default);
	}

	public static T CommonNode<T>(T first, T second, Func<T, T?> next) where T : struct
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		return CommonNodeExtracted(first, second, next, EqualityComparer<T>.Default);
	}

	public static T? CommonNode<T>(T first, T second, Func<T, T?> next, IEqualityComparer<T>? comparer) where T : class
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		if (comparer == null)
		{
			return CommonNodeExtracted(first, second, next, EqualityComparer<T>.Default);
		}
		return CommonNodeExtracted(first, second, next, comparer);
	}

	public static T CommonNode<T>(T first, T second, Func<T, T?> next, IEqualityComparer<T>? comparer) where T : struct
	{
		if (next == null)
		{
			throw new ArgumentNullException("next");
		}
		if (comparer == null)
		{
			return CommonNodeExtracted(first, second, next, EqualityComparer<T>.Default);
		}
		return CommonNodeExtracted(first, second, next, comparer);
	}

	private static T? CommonNodeExtracted<T>(T first, T second, Func<T, T?> next, IEqualityComparer<T> comparer) where T : class
	{
		if (comparer.Equals(first, second))
		{
			return first;
		}
		return ExploreSequenceUntilNull(first, next).Intersect(ExploreSequenceUntilNull(second, next), comparer).FirstOrDefault();
	}

	private static T CommonNodeExtracted<T>(T first, T second, Func<T, T?> next, IEqualityComparer<T> comparer) where T : struct
	{
		if (comparer.Equals(first, second))
		{
			return first;
		}
		return ExploreSequenceUntilNull(first, next).Intersect(ExploreSequenceUntilNull(second, next), comparer).FirstOrDefault();
	}
}
