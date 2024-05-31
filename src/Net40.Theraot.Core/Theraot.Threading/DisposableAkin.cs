using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Threading;

namespace Theraot.Threading;

[DebuggerNonUserCode]
public sealed class DisposableAkin : CriticalFinalizerObject, IDisposable
{
	private Action? _release;

	private StrongBox<UniqueId>? _threadUniqueId;

	public bool IsDisposed => _threadUniqueId == null;

	private DisposableAkin(Action release, UniqueId threadUniqueId)
	{
		_release = release ?? throw new ArgumentNullException("release");
		_threadUniqueId = new StrongBox<UniqueId>(threadUniqueId);
	}

	~DisposableAkin()
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

	public static DisposableAkin Create(Action release)
	{
		return new DisposableAkin(release, ThreadUniqueId.CurrentThreadId);
	}

	public bool Dispose(Func<bool> condition)
	{
		if (condition == null)
		{
			throw new ArgumentNullException("condition");
		}
		StrongBox<UniqueId> strongBox = Interlocked.CompareExchange(ref _threadUniqueId, null, new StrongBox<UniqueId>(ThreadUniqueId.CurrentThreadId));
		if (strongBox == null || strongBox.Value != ThreadUniqueId.CurrentThreadId)
		{
			return false;
		}
		if (!condition())
		{
			return false;
		}
		if (_release == null)
		{
			return true;
		}
		try
		{
			_release();
			return true;
		}
		finally
		{
			_release = null;
		}
	}

	[DebuggerNonUserCode]
	public void Dispose()
	{
		try
		{
			Dispose(disposeManagedResources: true);
		}
		finally
		{
			GC.SuppressFinalize(this);
		}
	}

	private void Dispose(bool disposeManagedResources)
	{
		if (disposeManagedResources)
		{
			StrongBox<UniqueId> strongBox = Interlocked.CompareExchange(ref _threadUniqueId, null, new StrongBox<UniqueId>(ThreadUniqueId.CurrentThreadId));
			if (strongBox == null || strongBox.Value != ThreadUniqueId.CurrentThreadId)
			{
				return;
			}
		}
		if (_release == null)
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
}
