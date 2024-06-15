using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Theraot.Threading;

namespace System.Threading;

public static class SemaphoreSlimTheraotExtensions
{
	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task WaitAsync(this SemaphoreSlim semaphore)
	{
		if (semaphore == null)
		{
			throw new NullReferenceException();
		}
		GC.KeepAlive(semaphore.AvailableWaitHandle);
		return WaitPrivateAsync(semaphore);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task WaitAsync(this SemaphoreSlim semaphore, CancellationToken cancellationToken)
	{
		if (semaphore == null)
		{
			throw new NullReferenceException();
		}
		GC.KeepAlive(semaphore.AvailableWaitHandle);
		return WaitPrivateAsync(semaphore, cancellationToken);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task<bool> WaitAsync(this SemaphoreSlim semaphore, int millisecondsTimeout)
	{
		if (semaphore == null)
		{
			throw new NullReferenceException();
		}
		GC.KeepAlive(semaphore.AvailableWaitHandle);
		if (millisecondsTimeout < -1)
		{
			throw new ArgumentOutOfRangeException("millisecondsTimeout");
		}
		return (millisecondsTimeout == -1) ? WaitPrivateAsync(semaphore) : WaitPrivateAsync(semaphore, millisecondsTimeout);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task<bool> WaitAsync(this SemaphoreSlim semaphore, TimeSpan timeout)
	{
		if (semaphore == null)
		{
			throw new NullReferenceException();
		}
		GC.KeepAlive(semaphore.AvailableWaitHandle);
		int num = (int)timeout.TotalMilliseconds;
		if (num < -1)
		{
			throw new ArgumentOutOfRangeException("timeout");
		}
		return (num == -1) ? WaitPrivateAsync(semaphore) : WaitPrivateAsync(semaphore, num);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task<bool> WaitAsync(this SemaphoreSlim semaphore, TimeSpan timeout, CancellationToken cancellationToken)
	{
		if (semaphore == null)
		{
			throw new NullReferenceException();
		}
		GC.KeepAlive(semaphore.AvailableWaitHandle);
		int num = (int)timeout.TotalMilliseconds;
		if (num < -1)
		{
			throw new ArgumentOutOfRangeException("timeout");
		}
		return (num == -1) ? WaitPrivateAsync(semaphore, cancellationToken) : WaitPrivateAsync(semaphore, (int)timeout.TotalMilliseconds, cancellationToken);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task<bool> WaitAsync(this SemaphoreSlim semaphore, int millisecondsTimeout, CancellationToken cancellationToken)
	{
		if (semaphore == null)
		{
			throw new NullReferenceException();
		}
		GC.KeepAlive(semaphore.AvailableWaitHandle);
		if (millisecondsTimeout < -1)
		{
			throw new ArgumentOutOfRangeException("millisecondsTimeout");
		}
		return (millisecondsTimeout == -1) ? WaitPrivateAsync(semaphore, cancellationToken) : WaitPrivateAsync(semaphore, millisecondsTimeout, cancellationToken);
	}

	private static async Task<bool> WaitPrivateAsync(SemaphoreSlim semaphore)
	{
		if (semaphore.Wait(0))
		{
			return true;
		}
		do
		{
			await TaskTheraotExtensions.ConfigureAwait(TaskExEx.FromWaitHandleInternal(semaphore.AvailableWaitHandle), continueOnCapturedContext: false);
		}
		while (!semaphore.Wait(0));
		return true;
	}

	private static async Task<bool> WaitPrivateAsync(SemaphoreSlim semaphore, int millisecondsTimeout)
	{
		if (semaphore.Wait(0))
		{
			return true;
		}
		long start = ThreadingHelper.TicksNow();
		int timeout = millisecondsTimeout;
		while (await TaskTheraotExtensions.ConfigureAwait(TaskExEx.FromWaitHandleInternal(semaphore.AvailableWaitHandle, timeout), continueOnCapturedContext: false))
		{
			if (semaphore.Wait(0))
			{
				return true;
			}
			timeout = (int)(millisecondsTimeout - ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start));
		}
		return false;
	}

	private static async Task<bool> WaitPrivateAsync(SemaphoreSlim semaphore, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (semaphore.Wait(0, cancellationToken))
		{
			return true;
		}
		do
		{
			await TaskTheraotExtensions.ConfigureAwait(TaskExEx.FromWaitHandleInternal(semaphore.AvailableWaitHandle, cancellationToken), continueOnCapturedContext: false);
		}
		while (!semaphore.Wait(0, cancellationToken));
		return true;
	}

	private static async Task<bool> WaitPrivateAsync(SemaphoreSlim semaphore, int millisecondsTimeout, CancellationToken cancellationToken)
	{
		if (semaphore.Wait(0, cancellationToken))
		{
			return true;
		}
		long start = ThreadingHelper.TicksNow();
		int timeout = millisecondsTimeout;
		while (true)
		{
			bool flag = timeout > 0;
			bool flag2 = flag;
			if (flag2)
			{
				flag2 = await TaskTheraotExtensions.ConfigureAwait(TaskExEx.FromWaitHandleInternal(semaphore.AvailableWaitHandle, timeout, cancellationToken), continueOnCapturedContext: false);
			}
			if (!flag2)
			{
				break;
			}
			if (semaphore.Wait(0, cancellationToken))
			{
				return true;
			}
			timeout = (int)(millisecondsTimeout - ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start));
		}
		return false;
	}
}