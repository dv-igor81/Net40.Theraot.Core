using System.Reflection;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices;

public readonly struct TaskAwaiter : ICriticalNotifyCompletion, INotifyCompletion
{
	private static readonly object[] _emptyParams = new object[0];

	private static readonly MethodInfo? _prepForRemoting = GetPrepForRemotingMethodInfo();

	private readonly Task _task;

	public bool IsCompleted => _task.IsCompleted;

	private static bool IsValidLocationForInlining
	{
		get
		{
			SynchronizationContext current = SynchronizationContext.Current;
			if (current != null && current.GetType() != typeof(SynchronizationContext))
			{
				return false;
			}
			return TaskScheduler.Current == TaskScheduler.Default;
		}
	}

	internal TaskAwaiter(Task task)
	{
		_task = task;
	}

	public void GetResult()
	{
		ValidateEnd(_task);
	}

	public void OnCompleted(Action continuation)
	{
		OnCompletedInternal(_task, continuation, continueOnCapturedContext: true);
	}

	[SecurityCritical]
	public void UnsafeOnCompleted(Action continuation)
	{
		OnCompletedInternal(_task, continuation, continueOnCapturedContext: true);
	}

	internal static void OnCompletedInternal(Task task, Action continuation, bool continueOnCapturedContext)
	{
		if (continuation == null)
		{
			throw new ArgumentNullException("continuation");
		}
		SynchronizationContext syncContext = (continueOnCapturedContext ? SynchronizationContext.Current : null);
		if (syncContext != null && syncContext.GetType() != typeof(SynchronizationContext))
		{
			task.ContinueWith(delegate
			{
				try
				{
					syncContext.Post(delegate(object state)
					{
						((Action)state)();
					}, continuation);
				}
				catch (Exception exception)
				{
					System.Runtime.CompilerServices.AsyncMethodBuilderCore.ThrowOnContext(exception, null);
				}
			}, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
		}
		else
		{
			OnCompletedWithoutSyncContext(task, continuation, continueOnCapturedContext);
		}
	}

	internal static Exception? PrepareExceptionForRethrow(Exception? exc)
	{
		if (_prepForRemoting == null)
		{
			return exc;
		}
		try
		{
			_prepForRemoting.Invoke(exc, _emptyParams);
		}
		catch (Exception)
		{
			// ignored
		}

		return exc;
	}

	internal static void ValidateEnd(Task task)
	{
		if (task.Status != TaskStatus.RanToCompletion)
		{
			HandleNonSuccess(task);
		}
	}

	private static MethodInfo? GetPrepForRemotingMethodInfo()
	{
		try
		{
			return typeof(Exception).GetMethod("PrepForRemoting", BindingFlags.Instance | BindingFlags.NonPublic);
		}
		catch (Exception)
		{
			return null;
		}
	}

	private static void HandleNonSuccess(Task task)
	{
		if (!task.IsCompleted)
		{
			try
			{
				task.Wait();
			}
			catch (Exception)
			{
			}
		}
		if (task.Status != TaskStatus.RanToCompletion)
		{
			ThrowForNonSuccess(task);
		}
	}

	private static void OnCompletedWithoutSyncContext(Task task, Action continuation, bool continueOnCapturedContext)
	{
		TaskScheduler taskScheduler = (continueOnCapturedContext ? TaskScheduler.Current : TaskScheduler.Default);
		if (task.IsCompleted)
		{
			Task.Factory.StartNew(delegate(object state)
			{
				((Action)state)?.Invoke();
			}, continuation, CancellationToken.None, TaskCreationOptions.None, taskScheduler);
		}
		else if (taskScheduler != TaskScheduler.Default)
		{
			task.ContinueWith(delegate
			{
				RunNoException(continuation);
			}, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, taskScheduler);
		}
		else
		{
			task.ContinueWith(ContinuationFunction, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
		}
		void ContinuationFunction(Task completedTask)
		{
			if (IsValidLocationForInlining)
			{
				RunNoException(continuation);
			}
			else
			{
				Task.Factory.StartNew(delegate(object state)
				{
					RunNoException((Action)state);
				}, continuation, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
			}
		}
	}

	private static void RunNoException(Action? continuation)
	{
		if (continuation == null)
		{
			return;
		}
		try
		{
			continuation();
		}
		catch (Exception exception)
		{
			AsyncMethodBuilderCore.ThrowOnContext(exception, null);
		}
	}

	private static void ThrowForNonSuccess(Task task)
	{
		switch (task.Status)
		{
		case TaskStatus.Canceled:
			throw new TaskCanceledException(task);
		case TaskStatus.Faulted:
			throw PrepareExceptionForRethrow(task.Exception.InnerException);
		case TaskStatus.WaitingForActivation:
			return;
		case TaskStatus.WaitingToRun:
			return;
		case TaskStatus.WaitingForChildrenToComplete:
			return;
		default:
			throw new InvalidOperationException("The task has not yet completed.");
		}
	}
}
public readonly struct TaskAwaiter<TResult> : ICriticalNotifyCompletion, INotifyCompletion
{
	private readonly Task<TResult> _task;

	public bool IsCompleted => _task.IsCompleted;

	internal TaskAwaiter(Task<TResult> task)
	{
		_task = task;
	}

	public TResult GetResult()
	{
		TaskAwaiter.ValidateEnd(_task);
		return _task.Result;
	}

	public void OnCompleted(Action continuation)
	{
		TaskAwaiter.OnCompletedInternal(_task, continuation, continueOnCapturedContext: true);
	}

	[SecurityCritical]
	public void UnsafeOnCompleted(Action continuation)
	{
		TaskAwaiter.OnCompletedInternal(_task, continuation, continueOnCapturedContext: true);
	}
}
