#define DEBUG
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace System.Runtime.CompilerServices;

[StructLayout(LayoutKind.Auto)]
public readonly struct ConfiguredValueTaskAwaitable
{
	[StructLayout(LayoutKind.Auto)]
	public readonly struct ConfiguredValueTaskAwaiter : ICriticalNotifyCompletion, INotifyCompletion
	{
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
		internal ConfiguredValueTaskAwaiter(ValueTask value)
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
				TaskTheraotExtensions.ConfigureAwait(task, _value._continueOnCapturedContext).GetAwaiter().OnCompleted(continuation);
			}
			else if (obj != null)
			{
				((IValueTaskSource)obj).OnCompleted(ValueTaskAwaiter.s_invokeActionDelegate, continuation, _value._token, ValueTaskSourceOnCompletedFlags.FlowExecutionContext | (_value._continueOnCapturedContext ? ValueTaskSourceOnCompletedFlags.UseSchedulingContext : ValueTaskSourceOnCompletedFlags.None));
			}
			else
			{
				TaskTheraotExtensions.ConfigureAwait(ValueTask.CompletedTask, _value._continueOnCapturedContext).GetAwaiter().OnCompleted(continuation);
			}
		}

		public void UnsafeOnCompleted(Action continuation)
		{
			object obj = _value._obj;
			Debug.Assert(obj == null || obj is Task || obj is IValueTaskSource);
			if (obj is Task task)
			{
				TaskTheraotExtensions.ConfigureAwait(task, _value._continueOnCapturedContext).GetAwaiter().UnsafeOnCompleted(continuation);
			}
			else if (obj != null)
			{
				((IValueTaskSource)obj).OnCompleted(ValueTaskAwaiter.s_invokeActionDelegate, continuation, _value._token, _value._continueOnCapturedContext ? ValueTaskSourceOnCompletedFlags.UseSchedulingContext : ValueTaskSourceOnCompletedFlags.None);
			}
			else
			{
				TaskTheraotExtensions.ConfigureAwait(ValueTask.CompletedTask, _value._continueOnCapturedContext).GetAwaiter().UnsafeOnCompleted(continuation);
			}
		}
	}

	private readonly ValueTask _value;

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	internal ConfiguredValueTaskAwaitable(ValueTask value)
	{
		_value = value;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public ConfiguredValueTaskAwaiter GetAwaiter()
	{
		return new ConfiguredValueTaskAwaiter(_value);
	}
}
[StructLayout(LayoutKind.Auto)]
public readonly struct ConfiguredValueTaskAwaitable<TResult>
{
	[StructLayout(LayoutKind.Auto)]
	public readonly struct ConfiguredValueTaskAwaiter : ICriticalNotifyCompletion, INotifyCompletion
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
		internal ConfiguredValueTaskAwaiter(ValueTask<TResult> value)
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
				TaskTheraotExtensions.ConfigureAwait(task, _value._continueOnCapturedContext).GetAwaiter().OnCompleted(continuation);
			}
			else if (obj != null)
			{
				((IValueTaskSource<TResult>)obj).OnCompleted(ValueTaskAwaiter.s_invokeActionDelegate, continuation, _value._token, ValueTaskSourceOnCompletedFlags.FlowExecutionContext | (_value._continueOnCapturedContext ? ValueTaskSourceOnCompletedFlags.UseSchedulingContext : ValueTaskSourceOnCompletedFlags.None));
			}
			else
			{
				TaskTheraotExtensions.ConfigureAwait(ValueTask.CompletedTask, _value._continueOnCapturedContext).GetAwaiter().OnCompleted(continuation);
			}
		}

		public void UnsafeOnCompleted(Action continuation)
		{
			object obj = _value._obj;
			Debug.Assert(obj == null || obj is Task<TResult> || obj is IValueTaskSource<TResult>);
			if (obj is Task<TResult> task)
			{
				TaskTheraotExtensions.ConfigureAwait(task, _value._continueOnCapturedContext).GetAwaiter().UnsafeOnCompleted(continuation);
			}
			else if (obj != null)
			{
				((IValueTaskSource<TResult>)obj).OnCompleted(ValueTaskAwaiter.s_invokeActionDelegate, continuation, _value._token, _value._continueOnCapturedContext ? ValueTaskSourceOnCompletedFlags.UseSchedulingContext : ValueTaskSourceOnCompletedFlags.None);
			}
			else
			{
				TaskTheraotExtensions.ConfigureAwait(ValueTask.CompletedTask, _value._continueOnCapturedContext).GetAwaiter().UnsafeOnCompleted(continuation);
			}
		}
	}

	private readonly ValueTask<TResult> _value;

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	internal ConfiguredValueTaskAwaitable(ValueTask<TResult> value)
	{
		_value = value;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public ConfiguredValueTaskAwaiter GetAwaiter()
	{
		return new ConfiguredValueTaskAwaiter(_value);
	}
}
