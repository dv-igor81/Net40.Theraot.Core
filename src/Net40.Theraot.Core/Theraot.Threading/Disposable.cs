using System;
using System.Diagnostics;
using System.Threading;
using Theraot.Core;

namespace Theraot.Threading;

[DebuggerNonUserCode]
public sealed class Disposable : IDisposable
{
	private Action? _release;

	private int _disposeStatus;

	public bool IsDisposed
	{
		[DebuggerNonUserCode]
		get
		{
			return _disposeStatus == -1;
		}
	}

	private Disposable(Action release)
	{
		_release = release ?? throw new ArgumentNullException("release");
	}

	public static Disposable Create(Action release)
	{
		return new Disposable(release);
	}

	public bool Dispose(Func<bool> condition)
	{
		if (condition == null)
		{
			throw new ArgumentNullException("condition");
		}
		return DisposedConditional(FuncHelper.GetFallacyFunc(), delegate
		{
			if (!condition())
			{
				return false;
			}
			Dispose();
			return true;
		});
	}

	[DebuggerNonUserCode]
	~Disposable()
	{
		try
		{
		}
		finally
		{
			try
			{
				Dispose(disposeManagedResources: false);
			}
			catch (Exception)
			{
			}
		}
	}

	[DebuggerNonUserCode]
	public void Dispose()
	{
		Dispose(disposeManagedResources: true);
		GC.SuppressFinalize(this);
	}

	[DebuggerNonUserCode]
	public void DisposedConditional(Action whenDisposed, Action whenNotDisposed)
	{
		if (_disposeStatus == -1)
		{
			whenDisposed?.Invoke();
		}
		else
		{
			if (whenNotDisposed == null)
			{
				return;
			}
			if (ThreadingHelper.SpinWaitRelativeSet(ref _disposeStatus, 1, -1))
			{
				try
				{
					whenNotDisposed();
					return;
				}
				finally
				{
					Interlocked.Decrement(ref _disposeStatus);
				}
			}
			whenDisposed?.Invoke();
		}
	}

	[DebuggerNonUserCode]
	public TReturn DisposedConditional<TReturn>(Func<TReturn> whenDisposed, Func<TReturn> whenNotDisposed)
	{
		if (_disposeStatus == -1)
		{
			return (whenDisposed == null) ? default(TReturn) : whenDisposed();
		}
		if (whenNotDisposed == null)
		{
			return default(TReturn);
		}
		if (!ThreadingHelper.SpinWaitRelativeSet(ref _disposeStatus, 1, -1))
		{
			return (whenDisposed == null) ? default(TReturn) : whenDisposed();
		}
		try
		{
			return whenNotDisposed();
		}
		finally
		{
			Interlocked.Decrement(ref _disposeStatus);
		}
	}

	[DebuggerNonUserCode]
	private void Dispose(bool disposeManagedResources)
	{
		if (!TakeDisposalExecution() || _release == null)
		{
			return;
		}
		try
		{
			_release();
		}
		finally
		{
			_release = null;
		}
	}

	private bool TakeDisposalExecution()
	{
		return _disposeStatus != -1 && ThreadingHelper.SpinWaitSetUnless(ref _disposeStatus, -1, 0, -1);
	}
}
