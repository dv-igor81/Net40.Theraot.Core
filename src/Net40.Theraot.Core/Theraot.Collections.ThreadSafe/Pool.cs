using System;
using Theraot.Threading;

namespace Theraot.Collections.ThreadSafe;

internal sealed class Pool<T> where T : class
{
	private readonly FixedSizeQueue<T> _entries;

	private readonly Action<T>? _recycler;

	private readonly UniqueId _reentryGuardId;

	public Pool(int capacity, Action<T>? recycler)
	{
		_reentryGuardId = RuntimeUniqueIdProvider.GetNextId();
		_entries = new FixedSizeQueue<T>(capacity);
		_recycler = recycler;
	}

	internal void Donate(T? entry)
	{
		if (entry != null && ReentryGuard.Enter(_reentryGuardId))
		{
			try
			{
				FixedSizeQueue<T> entries = _entries;
				Action<T> recycler = _recycler;
				if (entries != null && recycler != null)
				{
					recycler(entry);
					entries.TryAdd(entry);
				}
				return;
			}
			catch (ObjectDisposedException)
			{
			}
			catch (InvalidOperationException)
			{
			}
			catch (NullReferenceException)
			{
			}
			finally
			{
				ReentryGuard.Leave(_reentryGuardId);
			}
		}
		if (entry is IDisposable disposable)
		{
			disposable.Dispose();
		}
	}

	internal bool TryGet(out T result)
	{
		return _entries.TryTake(out result);
	}
}
