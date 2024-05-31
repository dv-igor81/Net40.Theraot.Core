using System.Runtime.CompilerServices;
using Theraot;

namespace System.Threading.Tasks;

public static class TaskTheraotExtensions
{
	private const TaskContinuationOptions _conditionMask = TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion;

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static ConfiguredTaskAwaitable<TResult> ConfigureAwait<TResult>(this Task<TResult> task, bool continueOnCapturedContext)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		return new ConfiguredTaskAwaitable<TResult>(task, continueOnCapturedContext);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static ConfiguredTaskAwaitable ConfigureAwait(this Task task, bool continueOnCapturedContext)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		return new ConfiguredTaskAwaitable(task, continueOnCapturedContext);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static TaskAwaiter GetAwaiter(this Task task)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		return new TaskAwaiter(task);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static TaskAwaiter<TResult> GetAwaiter<TResult>(this Task<TResult> task)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		return new TaskAwaiter<TResult>(task);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		if (continuationAction == null)
		{
			throw new ArgumentNullException("continuationAction");
		}
		return ContinueWithExtracted(task, continuationAction, state, ref continuationOptions, scheduler, cancellationToken);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state, TaskScheduler scheduler)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		if (continuationAction == null)
		{
			throw new ArgumentNullException("continuationAction");
		}
		TaskContinuationOptions continuationOptions = TaskContinuationOptions.None;
		CancellationToken none = CancellationToken.None;
		return ContinueWithExtracted(task, continuationAction, state, ref continuationOptions, scheduler, none);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state, CancellationToken cancellationToken)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		if (continuationAction == null)
		{
			throw new ArgumentNullException("continuationAction");
		}
		TaskContinuationOptions continuationOptions = TaskContinuationOptions.None;
		return ContinueWithExtracted(task, continuationAction, state, ref continuationOptions, TaskScheduler.Current, cancellationToken);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state, TaskContinuationOptions continuationOptions)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		if (continuationAction == null)
		{
			throw new ArgumentNullException("continuationAction");
		}
		CancellationToken none = CancellationToken.None;
		return ContinueWithExtracted(task, continuationAction, state, ref continuationOptions, TaskScheduler.Current, none);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		if (continuationAction == null)
		{
			throw new ArgumentNullException("continuationAction");
		}
		TaskContinuationOptions continuationOptions = TaskContinuationOptions.None;
		CancellationToken none = CancellationToken.None;
		return ContinueWithExtracted(task, continuationAction, state, ref continuationOptions, TaskScheduler.Current, none);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		if (continuationFunction == null)
		{
			throw new ArgumentNullException("continuationFunction");
		}
		return ContinueWithExtracted(task, continuationFunction, state, ref continuationOptions, scheduler, cancellationToken);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, TaskScheduler scheduler)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		if (continuationFunction == null)
		{
			throw new ArgumentNullException("continuationFunction");
		}
		TaskContinuationOptions continuationOptions = TaskContinuationOptions.None;
		CancellationToken none = CancellationToken.None;
		return ContinueWithExtracted(task, continuationFunction, state, ref continuationOptions, scheduler, none);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, CancellationToken cancellationToken)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		if (continuationFunction == null)
		{
			throw new ArgumentNullException("continuationFunction");
		}
		TaskContinuationOptions continuationOptions = TaskContinuationOptions.None;
		return ContinueWithExtracted(task, continuationFunction, state, ref continuationOptions, TaskScheduler.Current, cancellationToken);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, TaskContinuationOptions continuationOptions)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		if (continuationFunction == null)
		{
			throw new ArgumentNullException("continuationFunction");
		}
		CancellationToken none = CancellationToken.None;
		return ContinueWithExtracted(task, continuationFunction, state, ref continuationOptions, TaskScheduler.Current, none);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		if (continuationFunction == null)
		{
			throw new ArgumentNullException("continuationFunction");
		}
		TaskContinuationOptions continuationOptions = TaskContinuationOptions.None;
		CancellationToken none = CancellationToken.None;
		return ContinueWithExtracted(task, continuationFunction, state, ref continuationOptions, TaskScheduler.Current, none);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		if (continuationAction == null)
		{
			throw new ArgumentNullException("continuationAction");
		}
		return ContinueWithExtracted(task, continuationAction, state, ref continuationOptions, scheduler, cancellationToken);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, TaskScheduler scheduler)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		if (continuationAction == null)
		{
			throw new ArgumentNullException("continuationAction");
		}
		TaskContinuationOptions continuationOptions = TaskContinuationOptions.None;
		CancellationToken none = CancellationToken.None;
		return ContinueWithExtracted(task, continuationAction, state, ref continuationOptions, scheduler, none);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, CancellationToken cancellationToken)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		if (continuationAction == null)
		{
			throw new ArgumentNullException("continuationAction");
		}
		TaskContinuationOptions continuationOptions = TaskContinuationOptions.None;
		return ContinueWithExtracted(task, continuationAction, state, ref continuationOptions, TaskScheduler.Current, cancellationToken);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, TaskContinuationOptions continuationOptions)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		if (continuationAction == null)
		{
			throw new ArgumentNullException("continuationAction");
		}
		CancellationToken none = CancellationToken.None;
		return ContinueWithExtracted(task, continuationAction, state, ref continuationOptions, TaskScheduler.Current, none);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		if (continuationAction == null)
		{
			throw new ArgumentNullException("continuationAction");
		}
		TaskContinuationOptions continuationOptions = TaskContinuationOptions.None;
		CancellationToken none = CancellationToken.None;
		return ContinueWithExtracted(task, continuationAction, state, ref continuationOptions, TaskScheduler.Current, none);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task<TResult> ContinueWith<TResult>(this Task<TResult> task, Func<Task<TResult>, object, TResult> continuationFunction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		if (continuationFunction == null)
		{
			throw new ArgumentNullException("continuationFunction");
		}
		return ContinueWithExtracted(task, continuationFunction, state, ref continuationOptions, scheduler, cancellationToken);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task<TResult> ContinueWith<TResult>(this Task<TResult> task, Func<Task<TResult>, object, TResult> continuationFunction, object state, TaskScheduler scheduler)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		if (continuationFunction == null)
		{
			throw new ArgumentNullException("continuationFunction");
		}
		TaskContinuationOptions continuationOptions = TaskContinuationOptions.None;
		CancellationToken none = CancellationToken.None;
		return ContinueWithExtracted(task, continuationFunction, state, ref continuationOptions, scheduler, none);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task<TResult> ContinueWith<TResult>(this Task<TResult> task, Func<Task<TResult>, object, TResult> continuationFunction, object state, CancellationToken cancellationToken)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		if (continuationFunction == null)
		{
			throw new ArgumentNullException("continuationFunction");
		}
		TaskContinuationOptions continuationOptions = TaskContinuationOptions.None;
		return ContinueWithExtracted(task, continuationFunction, state, ref continuationOptions, TaskScheduler.Current, cancellationToken);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task<TResult> ContinueWith<TResult>(this Task<TResult> task, Func<Task<TResult>, object, TResult> continuationFunction, object state, TaskContinuationOptions continuationOptions)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		if (continuationFunction == null)
		{
			throw new ArgumentNullException("continuationFunction");
		}
		CancellationToken none = CancellationToken.None;
		return ContinueWithExtracted(task, continuationFunction, state, ref continuationOptions, TaskScheduler.Current, none);
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	public static Task<TResult> ContinueWith<TResult>(this Task<TResult> task, Func<Task<TResult>, object, TResult> continuationFunction, object state)
	{
		if (task == null)
		{
			throw new NullReferenceException();
		}
		if (continuationFunction == null)
		{
			throw new ArgumentNullException("continuationFunction");
		}
		TaskContinuationOptions continuationOptions = TaskContinuationOptions.None;
		CancellationToken none = CancellationToken.None;
		return ContinueWithExtracted(task, continuationFunction, state, ref continuationOptions, TaskScheduler.Current, none);
	}

	private static Task ContinueWithExtracted(Task task, Action<Task, object> continuationAction, object state, ref TaskContinuationOptions continuationOptions, TaskScheduler scheduler, CancellationToken cancellationToken)
	{
		TaskCompletionSource<VoidStruct> source = new TaskCompletionSource<VoidStruct>(state);
		TaskContinuationOptions condition = RemoveConditions(ref continuationOptions);
		if (cancellationToken.CanBeCanceled)
		{
			RegisterCancellation(source, ref cancellationToken);
			if (cancellationToken.IsCancellationRequested)
			{
				return source.Task;
			}
		}
		task.ContinueWith(delegate(Task t)
		{
			if (ValidateConditions(t, condition))
			{
				try
				{
					continuationAction(t, state);
					source.TrySetResult(default(VoidStruct));
					return;
				}
				catch (Exception exception)
				{
					source.TrySetException(exception);
					return;
				}
			}
			source.TrySetCanceled();
		}, cancellationToken, continuationOptions, scheduler);
		return source.Task;
	}

	private static Task ContinueWithExtracted<TResult>(Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, ref TaskContinuationOptions continuationOptions, TaskScheduler scheduler, CancellationToken cancellationToken)
	{
		TaskCompletionSource<VoidStruct> source = new TaskCompletionSource<VoidStruct>(state);
		TaskContinuationOptions condition = RemoveConditions(ref continuationOptions);
		if (cancellationToken.CanBeCanceled)
		{
			RegisterCancellation(source, ref cancellationToken);
			if (cancellationToken.IsCancellationRequested)
			{
				return source.Task;
			}
		}
		task.ContinueWith(delegate(Task<TResult> t)
		{
			if (ValidateConditions(t, condition))
			{
				try
				{
					continuationAction(t, state);
					source.TrySetResult(default(VoidStruct));
					return;
				}
				catch (Exception exception)
				{
					source.TrySetException(exception);
					return;
				}
			}
			source.TrySetCanceled();
		}, cancellationToken, continuationOptions, scheduler);
		return source.Task;
	}

	private static Task<TResult> ContinueWithExtracted<TResult>(Task task, Func<Task, object, TResult> continuationFunction, object state, ref TaskContinuationOptions continuationOptions, TaskScheduler scheduler, CancellationToken cancellationToken)
	{
		TaskCompletionSource<TResult> source = new TaskCompletionSource<TResult>(state);
		TaskContinuationOptions condition = RemoveConditions(ref continuationOptions);
		if (cancellationToken.CanBeCanceled)
		{
			RegisterCancellation(source, ref cancellationToken);
			if (cancellationToken.IsCancellationRequested)
			{
				return source.Task;
			}
		}
		task.ContinueWith(delegate(Task t)
		{
			if (ValidateConditions(t, condition))
			{
				try
				{
					source.TrySetResult(continuationFunction(t, state));
					return;
				}
				catch (Exception exception)
				{
					source.TrySetException(exception);
					return;
				}
			}
			source.TrySetCanceled();
		}, cancellationToken, continuationOptions, scheduler);
		return source.Task;
	}

	private static Task<TResult> ContinueWithExtracted<TResult>(Task<TResult> task, Func<Task<TResult>, object, TResult> continuationFunction, object state, ref TaskContinuationOptions continuationOptions, TaskScheduler scheduler, CancellationToken cancellationToken)
	{
		TaskCompletionSource<TResult> source = new TaskCompletionSource<TResult>(state);
		TaskContinuationOptions condition = RemoveConditions(ref continuationOptions);
		if (cancellationToken.CanBeCanceled)
		{
			RegisterCancellation(source, ref cancellationToken);
			if (cancellationToken.IsCancellationRequested)
			{
				return source.Task;
			}
		}
		task.ContinueWith(delegate(Task<TResult> t)
		{
			if (ValidateConditions(t, condition))
			{
				try
				{
					source.TrySetResult(continuationFunction(t, state));
					return;
				}
				catch (Exception exception)
				{
					source.TrySetException(exception);
					return;
				}
			}
			source.TrySetCanceled();
		}, cancellationToken, continuationOptions, scheduler);
		return source.Task;
	}

	private static void RegisterCancellation<TResult>(TaskCompletionSource<TResult> source, ref CancellationToken cancellationToken)
	{
		cancellationToken.Register(delegate
		{
			source.TrySetCanceled();
		});
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	private static TaskContinuationOptions RemoveConditions(ref TaskContinuationOptions continuationOptions)
	{
		TaskContinuationOptions result = continuationOptions & (TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion);
		continuationOptions &= ~(TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion);
		return result;
	}

	[MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
	private static bool ValidateConditions(Task task, TaskContinuationOptions condition)
	{
		return task.Status switch
		{
			TaskStatus.RanToCompletion => (condition & TaskContinuationOptions.NotOnRanToCompletion) == 0, 
			TaskStatus.Canceled => (condition & TaskContinuationOptions.NotOnCanceled) == 0, 
			TaskStatus.Faulted => (condition & TaskContinuationOptions.NotOnFaulted) == 0, 
			_ => false, 
		};
	}
}
