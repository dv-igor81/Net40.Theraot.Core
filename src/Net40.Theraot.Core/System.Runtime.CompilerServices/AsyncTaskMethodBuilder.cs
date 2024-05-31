using System.Diagnostics;
using System.Security;
using System.Threading.Tasks;
using Theraot;

namespace System.Runtime.CompilerServices;

public struct AsyncTaskMethodBuilder : IAsyncMethodBuilder
{
	private static readonly TaskCompletionSource<VoidStruct> _cachedCompleted = AsyncTaskMethodBuilder<VoidStruct>.DefaultResultTask;

	private AsyncTaskMethodBuilder<VoidStruct> _builder;

	public readonly Task Task => _builder.Task;

	public static AsyncTaskMethodBuilder Create()
	{
		return default(AsyncTaskMethodBuilder);
	}

	public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
	{
		_builder.AwaitOnCompleted(ref awaiter, ref stateMachine);
	}

	public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
	{
		_builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
	}

	readonly void IAsyncMethodBuilder.PreBoxInitialization()
	{
		GC.KeepAlive(Task);
	}

	public void SetException(Exception exception)
	{
		_builder.SetException(exception);
	}

	public void SetResult()
	{
		_builder.SetResult(_cachedCompleted);
	}

	public void SetStateMachine(IAsyncStateMachine stateMachine)
	{
		_builder.SetStateMachine(stateMachine);
	}

	[DebuggerStepThrough]
	public readonly void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
	{
		_builder.Start(ref stateMachine);
	}

	internal readonly void SetNotificationForWaitCompletion(bool enabled)
	{
		_builder.SetNotificationForWaitCompletion(enabled);
	}
}
public struct AsyncTaskMethodBuilder<TResult> : IAsyncMethodBuilder
{
	internal static readonly TaskCompletionSource<TResult> DefaultResultTask;

	private System.Runtime.CompilerServices.AsyncMethodBuilderCore _coreState;

	private TaskCompletionSource<TResult>? _task;

	public Task<TResult> Task => CompletionSource.Task;

	internal TaskCompletionSource<TResult> CompletionSource
	{
		get
		{
			TaskCompletionSource<TResult> taskCompletionSource = _task;
			if (taskCompletionSource == null)
			{
				taskCompletionSource = (_task = new TaskCompletionSource<TResult>());
			}
			return taskCompletionSource;
		}
	}

	static AsyncTaskMethodBuilder()
	{
		DefaultResultTask = AsyncMethodTaskCache.CreateCompleted(default(TResult));
		AsyncVoidMethodBuilder.PreventUnobservedTaskExceptions();
	}

	public static AsyncTaskMethodBuilder<TResult> Create()
	{
		return default(AsyncTaskMethodBuilder<TResult>);
	}

	public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
	{
		try
		{
			Action completionAction = _coreState.GetCompletionAction(ref this, ref stateMachine);
			awaiter.OnCompleted(completionAction);
		}
		catch (Exception exception)
		{
			System.Runtime.CompilerServices.AsyncMethodBuilderCore.ThrowOnContext(exception, null);
		}
	}

	[SecuritySafeCritical]
	public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
	{
		try
		{
			Action completionAction = _coreState.GetCompletionAction(ref this, ref stateMachine);
			awaiter.UnsafeOnCompleted(completionAction);
		}
		catch (Exception exception)
		{
			System.Runtime.CompilerServices.AsyncMethodBuilderCore.ThrowOnContext(exception, null);
		}
	}

	void IAsyncMethodBuilder.PreBoxInitialization()
	{
		GC.KeepAlive(Task);
	}

	public void SetException(Exception exception)
	{
		if (exception == null)
		{
			throw new ArgumentNullException("exception");
		}
		TaskCompletionSource<TResult> completionSource = CompletionSource;
		if (!((exception is OperationCanceledException) ? completionSource.TrySetCanceled() : completionSource.TrySetException(exception)))
		{
			throw new InvalidOperationException("The Task was already completed.");
		}
	}

	public void SetResult(TResult result)
	{
		TaskCompletionSource<TResult> task = _task;
		if (task == null)
		{
			_task = AsyncMethodTaskCache.CreateCompleted(result);
		}
		else if (!task.TrySetResult(result))
		{
			throw new InvalidOperationException("The Task was already completed.");
		}
	}

	public void SetStateMachine(IAsyncStateMachine stateMachine)
	{
		_coreState.SetStateMachine(stateMachine);
	}

	[DebuggerStepThrough]
	public readonly void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
	{
		System.Runtime.CompilerServices.AsyncMethodBuilderCore.Start(ref stateMachine);
	}

	internal readonly void SetNotificationForWaitCompletion(bool enabled)
	{
	}

	internal void SetResult(TaskCompletionSource<TResult> completedTask)
	{
		if (_task == null)
		{
			_task = completedTask;
		}
		else
		{
			SetResult(default(TResult));
		}
	}
}
