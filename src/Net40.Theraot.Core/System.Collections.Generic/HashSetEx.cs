using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Threading;

namespace System.Collections.Generic;

[Serializable]
[DebuggerNonUserCode]
[DebuggerDisplay("Count={Count}")]
public class HashSetEx<T> : HashSet<T>, IReadOnlySet<T>, IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable
{
	private sealed class SpyEqualityComparer : IEqualityComparer<T>
	{
		private readonly ThreadLocal<Action<T, T>> _callback = new ThreadLocal<Action<T, T>>();

		private readonly IEqualityComparer<T> _wrapped;

		private SpyEqualityComparer(IEqualityComparer<T> wrapped)
		{
			_wrapped = wrapped;
		}

		public static IEqualityComparer<T> GetFrom(IEqualityComparer<T>? comparer)
		{
			if (comparer == null)
			{
				return new SpyEqualityComparer(EqualityComparer<T>.Default);
			}
			if (comparer is SpyEqualityComparer)
			{
				return comparer;
			}
			return new SpyEqualityComparer(comparer);
		}

		public bool Equals(T x, T y)
		{
			GetCallback()?.Invoke(x, y);
			return _wrapped.Equals(x, y);
		}

		public int GetHashCode(T obj)
		{
			return _wrapped.GetHashCode(obj);
		}

		public void SetCallback(Action<T, T>? callback)
		{
			_callback.Value = callback;
		}

		private Action<T, T>? GetCallback()
		{
			if (!_callback.IsValueCreated || _callback.Value == null)
			{
				return null;
			}
			return _callback.Value;
		}
	}

	public HashSetEx()
		: this((IEqualityComparer<T>?)EqualityComparer<T>.Default)
	{
	}

	public HashSetEx(IEnumerable<T> collection)
		: this(collection, (IEqualityComparer<T>?)EqualityComparer<T>.Default)
	{
	}

	public HashSetEx(IEnumerable<T> collection, IEqualityComparer<T>? comparer)
		: base(collection, SpyEqualityComparer.GetFrom(comparer))
	{
	}

	public HashSetEx(IEqualityComparer<T>? comparer)
		: base(SpyEqualityComparer.GetFrom(comparer))
	{
	}

	protected HashSetEx(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public new bool TryGetValue(T equalValue, [MaybeNullWhen(false)] out T actualValue)
	{
		SpyEqualityComparer spyEqualityComparer = Comparer as SpyEqualityComparer;
		T found = equalValue;
		spyEqualityComparer.SetCallback(delegate(T stored, T check)
		{
			found = stored;
		});
		bool flag = Contains(equalValue);
		spyEqualityComparer.SetCallback(null);
		if (flag)
		{
			actualValue = found;
			return true;
		}
		actualValue = default(T);
		return false;
	}
}