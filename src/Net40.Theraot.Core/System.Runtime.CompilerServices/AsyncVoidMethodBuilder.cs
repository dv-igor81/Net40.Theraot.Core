using System.Diagnostics;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices;

public struct AsyncVoidMethodBuilder : IAsyncMethodBuilder
{
	private static int _preventUnobservedTaskExceptionsInvoked;

	private readonly SynchronizationContext _synchronizationContext;

	private AsyncMethodBuilderCore _coreState;

	static AsyncVoidMethodBuilder()
	{
		PreventUnobservedTaskExceptions();
	}

	private AsyncVoidMethodBuilder(SynchronizationContext synchronizationContext)
	{
		_synchronizationContext = synchronizationContext;
		synchronizationContext?.OperationStarted();
		_coreState = default(AsyncMethodBuilderCore);
	}

	public static AsyncVoidMethodBuilder Create()
	{
		return new AsyncVoidMethodBuilder(SynchronizationContext.Current);
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
			AsyncMethodBuilderCore.ThrowOnContext(exception, null);
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
			AsyncMethodBuilderCore.ThrowOnContext(exception, null);
		}
	}

	readonly void IAsyncMethodBuilder.PreBoxInitialization()
	{
	}

	public readonly void SetException(Exception exception)
	{
		if (exception == null)
		{
			throw new ArgumentNullException("exception");
		}
		if (_synchronizationContext != null)
		{
			try
			{
				AsyncMethodBuilderCore.ThrowOnContext(exception, _synchronizationContext);
				return;
			}
			finally
			{
				NotifySynchronizationContextOfCompletion();
			}
		}
		AsyncMethodBuilderCore.ThrowOnContext(exception, null);
	}

	public readonly void SetResult()
	{
		if (_synchronizationContext != null)
		{
			NotifySynchronizationContextOfCompletion();
		}
	}

	public void SetStateMachine(IAsyncStateMachine stateMachine)
	{
		_coreState.SetStateMachine(stateMachine);
	}

	[DebuggerStepThrough]
	public readonly void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
	{
		AsyncMethodBuilderCore.Start(ref stateMachine);
	}

	internal static void PreventUnobservedTaskExceptions()
	{
		try
		{
			if (Interlocked.CompareExchange(ref _preventUnobservedTaskExceptionsInvoked, 1, 0) == 0)
			{
				TaskScheduler.UnobservedTaskException += delegate(object _, UnobservedTaskExceptionEventArgs e)
				{
					e.SetObserved();
				};
			}
		}
		catch (Exception)
		{
		}
	}

	private readonly void NotifySynchronizationContextOfCompletion()
	{
		try
		{
			_synchronizationContext.OperationCompleted();
		}
		catch (Exception exception)
		{
			AsyncMethodBuilderCore.ThrowOnContext(exception, null);
		}
	}
}