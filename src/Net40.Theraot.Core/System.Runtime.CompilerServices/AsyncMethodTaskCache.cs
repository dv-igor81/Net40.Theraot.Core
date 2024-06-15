using System.Threading.Tasks;

namespace System.Runtime.CompilerServices;

internal static class AsyncMethodTaskCache
{
	private static class Singleton<TResult>
	{
		private static CacheGeneric<TResult>? _instance;

		public static CacheGeneric<TResult>? GetInstance()
		{
			return _instance;
		}

		public static void SetInstance(CacheGeneric<TResult>? value)
		{
			_instance = value;
		}
	}

	private sealed class CacheBool : CacheGeneric<bool>
	{
		private readonly TaskCompletionSource<bool> _false = FromResultStatic(result: false);

		private readonly TaskCompletionSource<bool> _true = FromResultStatic(result: true);

		public override TaskCompletionSource<bool> FromResult(bool result)
		{
			return result ? _true : _false;
		}
	}

	private abstract class CacheGeneric<TResult>
	{
		public static TaskCompletionSource<TResult> FromResultStatic(TResult result)
		{
			TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
			taskCompletionSource.TrySetResult(result);
			return taskCompletionSource;
		}

		public abstract TaskCompletionSource<TResult> FromResult(TResult result);
	}

	private sealed class CacheInt32 : CacheGeneric<int>
	{
		private const int _maxInt32ValueExclusive = 9;

		private const int _minInt32ValueInclusive = -1;

		private static readonly TaskCompletionSource<int>[] _int32Tasks = CreateInt32Tasks();

		public override TaskCompletionSource<int> FromResult(int result)
		{
			if (result < -1 || result >= 9)
			{
				return FromResultStatic(result);
			}
			return _int32Tasks[result - -1];
		}

		private static TaskCompletionSource<int>[] CreateInt32Tasks()
		{
			TaskCompletionSource<int>[] array = new TaskCompletionSource<int>[10];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = FromResultStatic(i - 1);
			}
			return array;
		}
	}

	static AsyncMethodTaskCache()
	{
		Singleton<bool>.SetInstance(new CacheBool());
		Singleton<int>.SetInstance(new CacheInt32());
	}

	internal static TaskCompletionSource<TResult> CreateCompleted<TResult>(TResult result)
	{
		CacheGeneric<TResult> instance = Singleton<TResult>.GetInstance();
		if (instance == null)
		{
			return CacheGeneric<TResult>.FromResultStatic(result);
		}
		return instance.FromResult(result);
	}
}