#define DEBUG
using System.Diagnostics;
using System.Runtime.InteropServices;
using Theraot.Threading.Needles;

namespace System.Threading.Tasks.Sources;

[StructLayout(LayoutKind.Auto)]
public struct ManualResetValueTaskSourceCore<TResult>
{
	private Action<object?>? _continuation;

	private object? _continuationState;

	private ExecutionContext? _executionContext;

	private object? _capturedContext;

	private INeedle<TResult>? _result;

	public bool RunContinuationsAsynchronously { get; set; }

	public short Version { get; private set; }

	public void Reset()
	{
		Version++;
		_result = null;
		_capturedContext = null;
		_continuation = null;
		_continuationState = null;
		_executionContext = null;
	}

	public void SetResult(TResult result)
	{
		_result = new StructNeedle<TResult>(result);
		SignalCompletion();
	}

	public void SetException(Exception error)
	{
		_result = new ExceptionStructNeedle<TResult>(error);
		SignalCompletion();
	}

	public readonly ValueTaskSourceStatus GetStatus(short token)
	{
		ValidateToken(token);
		if (_continuation == null || _result == null)
		{
			return ValueTaskSourceStatus.Pending;
		}
		if (_result is ExceptionStructNeedle<TResult> exceptionStructNeedle)
		{
			return (exceptionStructNeedle.Exception is OperationCanceledException) ? ValueTaskSourceStatus.Canceled : ValueTaskSourceStatus.Faulted;
		}
		return ValueTaskSourceStatus.Succeeded;
	}

	public readonly TResult GetResult(short token)
	{
		ValidateToken(token);
		if (_result == null)
		{
			throw new InvalidOperationException();
		}
		return _result.Value;
	}

	public void OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags)
	{
		if (continuation == null)
		{
			throw new ArgumentNullException("continuation");
		}
		ValidateToken(token);
		if ((flags & ValueTaskSourceOnCompletedFlags.FlowExecutionContext) != 0)
		{
			_executionContext = ExecutionContext.Capture();
		}
		if ((flags & ValueTaskSourceOnCompletedFlags.UseSchedulingContext) != 0)
		{
			SynchronizationContext current = SynchronizationContext.Current;
			if (current != null && current.GetType() != typeof(SynchronizationContext))
			{
				_capturedContext = current;
			}
			else
			{
				TaskScheduler current2 = TaskScheduler.Current;
				if (current2 != TaskScheduler.Default)
				{
					_capturedContext = current2;
				}
			}
		}
		object obj = _continuation;
		if (obj == null)
		{
			_continuationState = state;
			obj = Interlocked.CompareExchange(ref _continuation, continuation, null);
		}
		if (obj == null)
		{
			return;
		}
		if (obj != ManualResetValueTaskSourceCore.Sentinel)
		{
			throw new InvalidOperationException();
		}
		object capturedContext = _capturedContext;
		object obj2 = capturedContext;
		if (obj2 != null)
		{
			if (!(obj2 is SynchronizationContext synchronizationContext))
			{
				if (obj2 is TaskScheduler scheduler)
				{
					Task.Factory.StartNew(continuation, state, CancellationToken.None, TaskCreationOptions.None, scheduler);
				}
			}
			else
			{
				synchronizationContext.Post(delegate(object s)
				{
					Tuple<Action<object>, object> tuple = (Tuple<Action<object>, object>)s;
					tuple.Item1(tuple.Item2);
				}, Tuple.Create(continuation, state));
			}
		}
		else if (_executionContext != null)
		{
			ThreadPoolEx.QueueUserWorkItem(continuation, state, preferLocal: true);
		}
		else
		{
			ThreadPoolEx.UnsafeQueueUserWorkItem(continuation, state, preferLocal: true);
		}
	}

	private readonly void ValidateToken(short token)
	{
		if (token != Version)
		{
			throw new InvalidOperationException();
		}
	}

	private void SignalCompletion()
	{
		if (_continuation == null && Interlocked.CompareExchange(ref _continuation, ManualResetValueTaskSourceCore.Sentinel, null) == null)
		{
			return;
		}
		if (_executionContext != null)
		{
			ExecutionContext.Run(_executionContext, delegate(object s)
			{
				((ManualResetValueTaskSourceCore<TResult>)s).InvokeContinuation();
			}, this);
		}
		else
		{
			InvokeContinuation();
		}
	}

	private readonly void InvokeContinuation()
	{
		Debug.Assert(_continuation != null);
		Action<object> continuation = _continuation;
		object capturedContext = _capturedContext;
		object obj = capturedContext;
		if (obj != null)
		{
			if (!(obj is SynchronizationContext synchronizationContext))
			{
				if (obj is TaskScheduler scheduler)
				{
					Task.Factory.StartNew(continuation, _continuationState, CancellationToken.None, TaskCreationOptions.None, scheduler);
				}
			}
			else
			{
				synchronizationContext.Post(delegate(object s)
				{
					Tuple<Action<object>, object> tuple = (Tuple<Action<object>, object>)s;
					tuple.Item1(tuple.Item2);
				}, Tuple.Create(continuation, _continuationState));
			}
		}
		else if (RunContinuationsAsynchronously)
		{
			if (_executionContext != null)
			{
				ThreadPoolEx.QueueUserWorkItem(continuation, _continuationState, preferLocal: true);
			}
			else
			{
				ThreadPoolEx.QueueUserWorkItem(continuation, _continuationState, preferLocal: true);
			}
		}
		else
		{
			continuation(_continuationState);
		}
	}
}
internal static class ManualResetValueTaskSourceCore
{
	internal static readonly Action<object?> Sentinel = CompletionSentinel;

	private static void CompletionSentinel(object? _)
	{
		DebugEx.Fail("The sentinel delegate should never be invoked.");
		throw new InvalidOperationException();
	}
}
