using System.Runtime.CompilerServices;
using Theraot.Threading;

namespace System.Threading;

public static class CancellationTokenSourceTheraotExtensions
{
	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static CancellationTokenSource CancelAfter(this CancellationTokenSource cancellationTokenSource, int millisecondsDelay)
	{
		if (cancellationTokenSource == null)
		{
			throw new NullReferenceException();
		}
		GC.KeepAlive(cancellationTokenSource.Token);
		RootedTimeout.Launch(delegate
		{
			try
			{
				cancellationTokenSource.Cancel();
			}
			catch (ObjectDisposedException)
			{
			}
		}, millisecondsDelay);
		return cancellationTokenSource;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static CancellationTokenSource CancelAfter(this CancellationTokenSource cancellationTokenSource, TimeSpan delay)
	{
		if (cancellationTokenSource == null)
		{
			throw new NullReferenceException();
		}
		GC.KeepAlive(cancellationTokenSource.Token);
		RootedTimeout.Launch(delegate
		{
			try
			{
				cancellationTokenSource.Cancel();
			}
			catch (ObjectDisposedException)
			{
			}
		}, delay);
		return cancellationTokenSource;
	}
}