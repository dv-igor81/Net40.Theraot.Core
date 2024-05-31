#define DEBUG
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace System.Runtime.CompilerServices;

public readonly struct ValueTaskAwaiter : ICriticalNotifyCompletion, INotifyCompletion
{
	internal static readonly Action<object> s_invokeActionDelegate = delegate(object state)
	{
		if (!(state is Action action))
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.state);
		}
		else
		{
			action();
		}
	};

	private readonly ValueTask _value;

	public bool IsCompleted
	{
		[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
		get
		{
			return _value.IsCompleted;
		}
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	internal ValueTaskAwaiter(ValueTask value)
	{
		_value = value;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	[StackTraceHidden]
	public void GetResult()
	{
		_value.ThrowIfCompletedUnsuccessfully();
	}

	public void OnCompleted(Action continuation)
	{
		object obj = _value._obj;
		Debug.Assert(obj == null || obj is Task || obj is IValueTaskSource);
		if (obj is Task task)
		{
			TaskTheraotExtensions.GetAwaiter(task).OnCompleted(continuation);
		}
		else if (obj != null)
		{
			((IValueTaskSource)obj).OnCompleted(s_invokeActionDelegate, continuation, _value._token, ValueTaskSourceOnCompletedFlags.UseSchedulingContext | ValueTaskSourceOnCompletedFlags.FlowExecutionContext);
		}
		else
		{
			TaskTheraotExtensions.GetAwaiter(ValueTask.CompletedTask).OnCompleted(continuation);
		}
	}

	public void UnsafeOnCompleted(Action continuation)
	{
		object obj = _value._obj;
		Debug.Assert(obj == null || obj is Task || obj is IValueTaskSource);
		if (obj is Task task)
		{
			TaskTheraotExtensions.GetAwaiter(task).UnsafeOnCompleted(continuation);
		}
		else if (obj != null)
		{
			((IValueTaskSource)obj).OnCompleted(s_invokeActionDelegate, continuation, _value._token, ValueTaskSourceOnCompletedFlags.UseSchedulingContext);
		}
		else
		{
			TaskTheraotExtensions.GetAwaiter(ValueTask.CompletedTask).UnsafeOnCompleted(continuation);
		}
	}
}
public readonly struct ValueTaskAwaiter<TResult> : ICriticalNotifyCompletion, INotifyCompletion
{
	private readonly ValueTask<TResult> _value;

	public bool IsCompleted
	{
		[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
		get
		{
			return _value.IsCompleted;
		}
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	internal ValueTaskAwaiter(ValueTask<TResult> value)
	{
		_value = value;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	[StackTraceHidden]
	public TResult GetResult()
	{
		return _value.Result;
	}

	public void OnCompleted(Action continuation)
	{
		object obj = _value._obj;
		Debug.Assert(obj == null || obj is Task<TResult> || obj is IValueTaskSource<TResult>);
		if (obj is Task<TResult> task)
		{
			TaskTheraotExtensions.GetAwaiter(task).OnCompleted(continuation);
		}
		else if (obj != null)
		{
			((IValueTaskSource<TResult>)obj).OnCompleted(ValueTaskAwaiter.s_invokeActionDelegate, continuation, _value._token, ValueTaskSourceOnCompletedFlags.UseSchedulingContext | ValueTaskSourceOnCompletedFlags.FlowExecutionContext);
		}
		else
		{
			TaskTheraotExtensions.GetAwaiter(ValueTask.CompletedTask).OnCompleted(continuation);
		}
	}

	public void UnsafeOnCompleted(Action continuation)
	{
		object obj = _value._obj;
		Debug.Assert(obj == null || obj is Task<TResult> || obj is IValueTaskSource<TResult>);
		if (obj is Task<TResult> task)
		{
			TaskTheraotExtensions.GetAwaiter(task).UnsafeOnCompleted(continuation);
		}
		else if (obj != null)
		{
			((IValueTaskSource<TResult>)obj).OnCompleted(ValueTaskAwaiter.s_invokeActionDelegate, continuation, _value._token, ValueTaskSourceOnCompletedFlags.UseSchedulingContext);
		}
		else
		{
			TaskTheraotExtensions.GetAwaiter(ValueTask.CompletedTask).UnsafeOnCompleted(continuation);
		}
	}
}
