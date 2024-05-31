using System;
using System.Threading;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Threading;

internal sealed class Timer : IDisposable
{
	private static readonly Pool<Timer> _pool = new Pool<Timer>(64, delegate(Timer time)
	{
		time.Stop();
	});

	private Action? _callback;

	private System.Threading.Timer? _timer;

	private Timer(Action callback, TimeSpan dueTime, TimeSpan period)
	{
		_callback = callback;
		_timer = new System.Threading.Timer(Callback, null, dueTime, period);
	}

	public static void Donate(ref Timer? timer)
	{
		_pool.Donate(Interlocked.Exchange(ref timer, null));
	}

	public static Timer GetTimer(Action callback, TimeSpan dueTime, TimeSpan period)
	{
		if (_pool.TryGet(out var result))
		{
			result.Change(callback, dueTime, period);
			return result;
		}
		return new Timer(callback, dueTime, period);
	}

	public void Change(Action callback, TimeSpan dueTime, TimeSpan period)
	{
		System.Threading.Timer timer = _timer;
		if (timer == null)
		{
			throw new ObjectDisposedException("Timer");
		}
		timer.Change(dueTime, period);
		_callback = callback;
	}

	void IDisposable.Dispose()
	{
		Interlocked.Exchange(ref _timer, null)?.Dispose();
		_callback = null;
	}

	public void Stop()
	{
		System.Threading.Timer timer = _timer;
		if (timer == null)
		{
			throw new ObjectDisposedException("Timer");
		}
		timer.Change(-1, -1);
		_callback = null;
	}

	private void Callback(object? state)
	{
		_callback?.Invoke();
	}
}
