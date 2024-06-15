using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Theraot.Collections;
using Theraot.Threading;

namespace System.Threading.Tasks;

public static class TaskEx
{
    private const string _argumentOutOfRangeTimeoutNonNegativeOrMinusOne =
        "The timeout must be non-negative or -1, and it must be less than or equal to Int32.MaxValue.";

    public static bool IsCompletedSuccessfully(this Task task)
    {
        // if (obj is Task<TResult> task)
        // {
        //     return task.Status == TaskStatus.RanToCompletion;
        // }

        return task.Status == TaskStatus.RanToCompletion;
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task<TResult> FromResult<TResult>(TResult result)
    {
        return TaskExEx.FromResult(result);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task Run(Action action)
    {
        return Run(action, CancellationToken.None);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task Run(Action action, CancellationToken cancellationToken)
    {
        return Task.Factory.StartNew(action, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task<TResult> Run<TResult>(Func<Task<TResult>> function)
    {
        return Run(function, CancellationToken.None);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task<TResult> Run<TResult>(Func<Task<TResult>> function, CancellationToken cancellationToken)
    {
        return Run<Task<TResult>>(function, cancellationToken).Unwrap();
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task Run(Func<Task> function)
    {
        return Run(function, CancellationToken.None);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task Run(Func<Task> function, CancellationToken cancellationToken)
    {
        return Run<Task>(function, cancellationToken).Unwrap();
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task<TResult> Run<TResult>(Func<TResult> function)
    {
        return Run(function, CancellationToken.None);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task<TResult> Run<TResult>(Func<TResult> function, CancellationToken cancellationToken)
    {
        return Task.Factory.StartNew(function, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task<Task<TResult>> WhenAny<TResult>(IEnumerable<Task<TResult>> tasks)
    {
        if (tasks == null)
        {
            throw new ArgumentNullException("tasks");
        }

        TaskCompletionSource<Task<TResult>> taskCompletionSource = new TaskCompletionSource<Task<TResult>>();
        Task.Factory.ContinueWhenAny(tasks.AsArrayInternal(),
            (Func<Task<TResult>, bool>)taskCompletionSource.TrySetResult, CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        return taskCompletionSource.Task;
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task<Task> WhenAny(IEnumerable<Task> tasks)
    {
        if (tasks == null)
        {
            throw new ArgumentNullException("tasks");
        }

        TaskCompletionSource<Task> taskCompletionSource = new TaskCompletionSource<Task>();
        Task.Factory.ContinueWhenAny(tasks.AsArrayInternal(), (Func<Task, bool>)taskCompletionSource.TrySetResult,
            CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        return taskCompletionSource.Task;
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task<Task> WhenAny(params Task[] tasks)
    {
        return WhenAny((IEnumerable<Task>)tasks);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task<Task<TResult>> WhenAny<TResult>(params Task<TResult>[] tasks)
    {
        return WhenAny((IEnumerable<Task<TResult>>)tasks);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static YieldAwaitable Yield()
    {
        return default(YieldAwaitable);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task Delay(int millisecondsDelay)
    {
        return Delay(millisecondsDelay, CancellationToken.None);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task Delay(int millisecondsDelay, CancellationToken cancellationToken)
    {
        if (millisecondsDelay < -1)
        {
            throw new ArgumentOutOfRangeException("millisecondsDelay",
                "The value needs to be either -1 (signifying an infinite timeout), 0 or a positive integer.");
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return TaskExEx.FromCanceled(cancellationToken);
        }

        if (millisecondsDelay == 0)
        {
            return TaskExEx.CompletedTask;
        }

        TaskCompletionSource<bool> source = new TaskCompletionSource<bool>();
        RootedTimeout.Launch(delegate { source.TrySetResult(result: true); }, delegate { source.TrySetCanceled(); },
            millisecondsDelay, cancellationToken);
        return source.Task;
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task Delay(TimeSpan millisecondsDelay)
    {
        return Delay(millisecondsDelay, CancellationToken.None);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task Delay(TimeSpan millisecondsDelay, CancellationToken cancellationToken)
    {
        long num = (long)millisecondsDelay.TotalMilliseconds;
        if (num < -1 || num > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException("millisecondsDelay",
                "The timeout must be non-negative or -1, and it must be less than or equal to Int32.MaxValue.");
        }

        return Delay((int)num, cancellationToken);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task WhenAll(params Task[] tasks)
    {
        return WhenAll((IEnumerable<Task>)tasks);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task<TResult[]> WhenAll<TResult>(params Task<TResult>[] tasks)
    {
        return WhenAll((IEnumerable<Task<TResult>>)tasks);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task WhenAll(IEnumerable<Task> tasks)
    {
        if (tasks == null)
        {
            throw new ArgumentNullException("tasks");
        }

        return WhenAllCore(tasks, delegate(Task[] _, TaskCompletionSource<object> tcs) { tcs.TrySetResult(null); });
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task<TResult[]> WhenAll<TResult>(IEnumerable<Task<TResult>> tasks)
    {
        if (tasks == null)
        {
            throw new ArgumentNullException("tasks");
        }

        return WhenAllCore(tasks, delegate(Task[] completedTasks, TaskCompletionSource<TResult[]> tcs)
        {
            tcs.TrySetResult((from Task<TResult> t in completedTasks
                select t.Result).ToArray());
        });
    }

    private static void AddPotentiallyUnwrappedExceptions(ref List<Exception>? targetList, Exception exception)
    {
        if (targetList == null)
        {
            targetList = new List<Exception>();
        }

        if (exception is AggregateException ex)
        {
            targetList.Add((ex.InnerExceptions.Count == 1) ? exception.InnerException : exception);
        }
        else
        {
            targetList.Add(exception);
        }
    }

    private static Task<TResult> WhenAllCore<TResult>(IEnumerable<Task> tasks,
        Action<Task[], TaskCompletionSource<TResult>> setResultAction)
    {
        TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
        Task[] array = tasks.AsArrayInternal();
        if (array.Length == 0)
        {
            setResultAction(array, tcs);
        }
        else
        {
            Task.Factory.ContinueWhenAll(array, delegate(Task[] completedTasks)
            {
                List<Exception> targetList = null;
                bool flag = false;
                foreach (Task task in completedTasks)
                {
                    if (task.IsFaulted)
                    {
                        AddPotentiallyUnwrappedExceptions(ref targetList, task.Exception);
                    }
                    else
                    {
                        flag |= task.IsCanceled;
                    }
                }

                if (targetList != null && targetList.Count > 0)
                {
                    tcs.TrySetException(targetList);
                }
                else if (flag)
                {
                    tcs.TrySetCanceled();
                }
                else
                {
                    setResultAction(completedTasks, tcs);
                }
            }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        return tcs.Task;
    }
}