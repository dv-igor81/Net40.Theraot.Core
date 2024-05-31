using System;
using System.Collections.Generic;
using System.Diagnostics;
using Theraot.Collections.ThreadSafe;
using Theraot.Reflection;
using Theraot.Threading.Needles;

namespace Theraot.Threading;

[DebuggerNonUserCode]
public sealed class ReentryGuard
{
	[ThreadStatic]
	private static HashSet<UniqueId>? _guard;

	private ThreadSafeQueue<Action>? _workQueue;

	public bool IsTaken => _guard?.Contains(Id) ?? false;

	internal UniqueId Id { get; }

	private ThreadSafeQueue<Action> WorkQueue => TypeHelper.LazyCreate(ref _workQueue, () => new ThreadSafeQueue<Action>());

	public ReentryGuard()
	{
		Id = RuntimeUniqueIdProvider.GetNextId();
	}

	public IPromise Execute(Action operation)
	{
		ThreadSafeQueue<Action> workQueue = WorkQueue;
		IPromise result = AddExecution(operation, workQueue);
		ExecutePending(workQueue, Id);
		return result;
	}

	public IPromise<T> Execute<T>(Func<T> operation)
	{
		ThreadSafeQueue<Action> workQueue = WorkQueue;
		IPromise<T> result = AddExecution(operation, WorkQueue);
		ExecutePending(workQueue, Id);
		return result;
	}

	public IDisposable TryEnter(out bool didEnter)
	{
		didEnter = Enter(Id);
		IDisposable result;
		if (!didEnter)
		{
			result = NoOpDisposable.Instance;
		}
		else
		{
			IDisposable disposable = DisposableAkin.Create(delegate
			{
				Leave(Id);
			});
			result = disposable;
		}
		return result;
	}

	internal static bool Enter(UniqueId id)
	{
		if (GCMonitor.FinalizingForUnload)
		{
			return false;
		}
		HashSet<UniqueId> guard = _guard;
		if (guard == null)
		{
			_guard = new HashSet<UniqueId> { id };
			return true;
		}
		if (guard.Contains(id))
		{
			return false;
		}
		guard.Add(id);
		return true;
	}

	internal static void Leave(UniqueId id)
	{
		_guard?.Remove(id);
	}

	private static IPromise AddExecution(Action action, ThreadSafeQueue<Action> queue)
	{
		Promise promised = new Promise(done: false);
		ReadOnlyPromise result = new ReadOnlyPromise(promised);
		queue.Add(delegate
		{
			try
			{
				action();
				promised.SetCompleted();
			}
			catch (Exception error)
			{
				promised.SetError(error);
			}
		});
		return result;
	}

	private static IPromise<T> AddExecution<T>(Func<T> action, ThreadSafeQueue<Action> queue)
	{
		PromiseNeedle<T> promised = new PromiseNeedle<T>(done: false);
		ReadOnlyPromiseNeedle<T> result = new ReadOnlyPromiseNeedle<T>(promised);
		queue.Add(delegate
		{
			try
			{
				promised.Value = action();
			}
			catch (Exception error)
			{
				promised.SetError(error);
			}
		});
		return result;
	}

	private static void ExecutePending(ThreadSafeQueue<Action> queue, UniqueId id)
	{
		bool flag = false;
		try
		{
			flag = Enter(id);
			if (flag)
			{
				Action item;
				while (queue.TryTake(out item))
				{
					item();
				}
			}
		}
		finally
		{
			if (flag)
			{
				Leave(id);
			}
		}
	}
}
