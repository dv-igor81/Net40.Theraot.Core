using System.Runtime.CompilerServices;
using System.Security;

namespace System.Threading;

public static class ThreadPoolEx
{
	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool QueueUserWorkItem(WaitCallback callBack)
	{
		return ThreadPool.QueueUserWorkItem(callBack);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool QueueUserWorkItem(WaitCallback callBack, object? state)
	{
		return ThreadPool.QueueUserWorkItem(callBack, state);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static bool QueueUserWorkItem(Action<object?> callBack, object? state, bool preferLocal)
	{
		if (callBack == null)
		{
			throw new ArgumentNullException("callBack");
		}
		return ThreadPool.QueueUserWorkItem(delegate(object obj)
		{
			callBack(obj);
		}, state);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	[SecurityCritical]
	public static bool UnsafeQueueUserWorkItem(WaitCallback callBack, object? state)
	{
		return ThreadPool.UnsafeQueueUserWorkItem(callBack, state);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	[SecurityCritical]
	public static bool UnsafeQueueUserWorkItem(Action<object?> callBack, object? state, bool preferLocal)
	{
		if (callBack == null)
		{
			throw new ArgumentNullException("callBack");
		}
		return ThreadPool.UnsafeQueueUserWorkItem(delegate(object obj)
		{
			callBack(obj);
		}, state);
	}
}
