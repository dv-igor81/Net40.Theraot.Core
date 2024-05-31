using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Threading.Needles;

[DebuggerNonUserCode]
public class Promise : IWaitablePromise, IPromise, INotifyCompletion, IRecyclable
{
	private readonly int _hashCode;

	private readonly StrongDelegateCollection _onCompleted;

	public Exception? Exception { get; private set; }

	public bool IsCompleted => WaitHandle?.IsSet ?? true;

	public bool IsFaulted => Exception != null;

	protected ManualResetEventSlim? WaitHandle { get; private set; }

	public Promise(bool done)
	{
		Exception = null;
		_hashCode = base.GetHashCode();
		if (!done)
		{
			WaitHandle = new ManualResetEventSlim(initialState: false);
		}
		_onCompleted = new StrongDelegateCollection(freeReentry: true);
	}

	public Promise(Exception exception)
	{
		Exception = exception ?? throw new ArgumentNullException("exception");
		_hashCode = Exception.GetHashCode();
		WaitHandle = new ManualResetEventSlim(initialState: true);
		_onCompleted = new StrongDelegateCollection(freeReentry: true);
	}

	~Promise()
	{
		ReleaseWaitHandle(done: false);
	}

	public virtual void Free()
	{
		ManualResetEventSlim waitHandle = WaitHandle;
		if (waitHandle == null)
		{
			WaitHandle = new ManualResetEventSlim(initialState: false);
		}
		else
		{
			waitHandle.Reset();
		}
		Exception = null;
	}

	public virtual void Free(Action beforeFree)
	{
		if (beforeFree == null)
		{
			throw new ArgumentNullException("beforeFree");
		}
		ManualResetEventSlim waitHandle = WaitHandle;
		if (waitHandle == null || waitHandle.IsSet)
		{
			try
			{
				beforeFree();
				return;
			}
			finally
			{
				if (waitHandle == null)
				{
					WaitHandle = new ManualResetEventSlim(initialState: false);
				}
				else
				{
					waitHandle.Reset();
				}
				Exception = null;
			}
		}
		waitHandle.Reset();
		Exception = null;
	}

	public override int GetHashCode()
	{
		return _hashCode;
	}

	public void OnCompleted(Action continuation)
	{
		_onCompleted.Add(continuation);
	}

	public void SetCompleted()
	{
		Exception = null;
		ReleaseWaitHandle(done: true);
	}

	public void SetError(Exception error)
	{
		Exception = error;
		ReleaseWaitHandle(done: true);
	}

	public override string ToString()
	{
		return (!IsCompleted) ? "[Not Created]" : (Exception?.ToString() ?? "[Done]");
	}

	public virtual void Wait()
	{
		ManualResetEventSlim waitHandle = WaitHandle;
		if (waitHandle == null)
		{
			return;
		}
		try
		{
			waitHandle.Wait();
		}
		catch (ObjectDisposedException)
		{
		}
	}

	public virtual void Wait(CancellationToken cancellationToken)
	{
		ManualResetEventSlim waitHandle = WaitHandle;
		if (waitHandle == null)
		{
			return;
		}
		try
		{
			waitHandle.Wait(cancellationToken);
		}
		catch (ObjectDisposedException)
		{
		}
	}

	public virtual void Wait(int milliseconds)
	{
		ManualResetEventSlim waitHandle = WaitHandle;
		if (waitHandle == null)
		{
			return;
		}
		try
		{
			waitHandle.Wait(milliseconds);
		}
		catch (ObjectDisposedException)
		{
		}
	}

	public virtual void Wait(TimeSpan timeout)
	{
		ManualResetEventSlim waitHandle = WaitHandle;
		if (waitHandle == null)
		{
			return;
		}
		try
		{
			waitHandle.Wait(timeout);
		}
		catch (ObjectDisposedException)
		{
		}
	}

	public virtual void Wait(int milliseconds, CancellationToken cancellationToken)
	{
		ManualResetEventSlim waitHandle = WaitHandle;
		if (waitHandle == null)
		{
			return;
		}
		try
		{
			waitHandle.Wait(milliseconds, cancellationToken);
		}
		catch (ObjectDisposedException)
		{
		}
	}

	public virtual void Wait(TimeSpan timeout, CancellationToken cancellationToken)
	{
		ManualResetEventSlim waitHandle = WaitHandle;
		if (waitHandle == null)
		{
			return;
		}
		try
		{
			waitHandle.Wait(timeout, cancellationToken);
		}
		catch (ObjectDisposedException)
		{
		}
	}

	protected void ReleaseWaitHandle(bool done)
	{
		ManualResetEventSlim waitHandle = WaitHandle;
		if (waitHandle != null)
		{
			if (done)
			{
				waitHandle.Set();
			}
			waitHandle.Dispose();
		}
		_onCompleted.Invoke(null, DelegateCollectionInvokeOptions.RemoveDelegates);
		WaitHandle = null;
	}
}
