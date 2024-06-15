using System.Runtime.CompilerServices;
using Theraot;

namespace System.Threading.Tasks;

public static class TaskExEx
{
    private sealed class WaitHandleCancellableTaskCompletionSourceManager
    {
        private readonly CancellationToken _cancellationToken;

        private readonly RegisteredWaitHandle[] _registeredWaitHandle;

        private readonly TaskCompletionSource<bool> _taskCompletionSource;

        private WaitHandleCancellableTaskCompletionSourceManager(CancellationToken cancellationToken,
            TaskCompletionSource<bool> taskCompletionSource)
        {
            _cancellationToken = cancellationToken;
            _taskCompletionSource = taskCompletionSource;
            _registeredWaitHandle = new RegisteredWaitHandle[1];
        }

        public static void CreateWithoutTimeout(WaitHandle waitHandle, CancellationToken cancellationToken,
            TaskCompletionSource<bool> taskCompletionSource)
        {
            WaitHandleCancellableTaskCompletionSourceManager result =
                new WaitHandleCancellableTaskCompletionSourceManager(cancellationToken, taskCompletionSource);
            result._registeredWaitHandle[0] = ThreadPool.RegisterWaitForSingleObject(waitHandle,
                result.CallbackWithoutTimeout, null, -1, executeOnlyOnce: true);
            cancellationToken.Register(delegate { result.Unregister(); });
        }

        public static void CreateWithTimeout(WaitHandle waitHandle, CancellationToken cancellationToken,
            TaskCompletionSource<bool> taskCompletionSource, int millisecondsTimeout)
        {
            WaitHandleCancellableTaskCompletionSourceManager result =
                new WaitHandleCancellableTaskCompletionSourceManager(cancellationToken, taskCompletionSource);
            result._registeredWaitHandle[0] = ThreadPool.RegisterWaitForSingleObject(waitHandle,
                result.CallbackWithTimeout, null, millisecondsTimeout, executeOnlyOnce: true);
            cancellationToken.Register(delegate { result.Unregister(); });
        }

        private void CallbackWithoutTimeout(object state, bool timeOut)
        {
            if (Unregister())
            {
                _taskCompletionSource.TrySetCanceled();
            }
            else
            {
                _taskCompletionSource.TrySetResult(result: true);
            }
        }

        private void CallbackWithTimeout(object state, bool timeOut)
        {
            if (!Unregister())
            {
                if (timeOut)
                {
                    _taskCompletionSource.TrySetResult(result: false);
                }
                else
                {
                    _taskCompletionSource.TrySetResult(result: true);
                }
            }
        }

        private bool Unregister()
        {
            Volatile.Read(ref _registeredWaitHandle[0]).Unregister(null);
            if (!_cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            _taskCompletionSource.TrySetCanceled();
            return true;
        }
    }

    private sealed class WaitHandleTaskCompletionSourceManager
    {
        private readonly RegisteredWaitHandle[] _registeredWaitHandle;

        private readonly TaskCompletionSource<bool> _taskCompletionSource;

        private WaitHandleTaskCompletionSourceManager(TaskCompletionSource<bool> taskCompletionSource)
        {
            _taskCompletionSource = taskCompletionSource;
            _registeredWaitHandle = new RegisteredWaitHandle[1];
        }

        public static void CreateWithoutTimeout(WaitHandle waitHandle, TaskCompletionSource<bool> taskCompletionSource)
        {
            WaitHandleTaskCompletionSourceManager waitHandleTaskCompletionSourceManager =
                new WaitHandleTaskCompletionSourceManager(taskCompletionSource);
            waitHandleTaskCompletionSourceManager._registeredWaitHandle[0] = ThreadPool.RegisterWaitForSingleObject(
                waitHandle, waitHandleTaskCompletionSourceManager.CallbackWithoutTimeout, null, -1,
                executeOnlyOnce: true);
        }

        public static void CreateWithTimeout(WaitHandle waitHandle, TaskCompletionSource<bool> taskCompletionSource,
            int millisecondsTimeout)
        {
            WaitHandleTaskCompletionSourceManager waitHandleTaskCompletionSourceManager =
                new WaitHandleTaskCompletionSourceManager(taskCompletionSource);
            waitHandleTaskCompletionSourceManager._registeredWaitHandle[0] = ThreadPool.RegisterWaitForSingleObject(
                waitHandle, waitHandleTaskCompletionSourceManager.CallbackWithTimeout, null, millisecondsTimeout,
                executeOnlyOnce: true);
        }

        private void CallbackWithoutTimeout(object state, bool timeOut)
        {
            Unregister();
            _taskCompletionSource.TrySetResult(result: true);
        }

        private void CallbackWithTimeout(object state, bool timeOut)
        {
            Unregister();
            if (timeOut)
            {
                _taskCompletionSource.TrySetResult(result: false);
            }
            else
            {
                _taskCompletionSource.TrySetResult(result: true);
            }
        }

        private void Unregister()
        {
            Volatile.Read(ref _registeredWaitHandle[0]).Unregister(null);
        }
    }

    private static Task _completedTask;

    public static Task CompletedTask
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        get
        {
            Task task = _completedTask;
            if (task == null)
            {
                task = (_completedTask = FromResult(default(VoidStruct)));
            }

            return task;
        }
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task FromCancellation(CancellationToken token)
    {
        return FromCancellation<VoidStruct>(token);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task<TResult> FromCancellation<TResult>(CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return FromCanceled<TResult>(token);
        }

        TaskCompletionSource<TResult> taskCompleteSource = new TaskCompletionSource<TResult>();
        if (token.CanBeCanceled)
        {
            token.Register(delegate
            {
                TaskCompletionSourceTheraotExtensions.TrySetCanceled(taskCompleteSource, token);
            });
        }

        return taskCompleteSource.Task;
    }

    public static Task FromWaitHandle(WaitHandle waitHandle)
    {
        if (waitHandle == null)
        {
            throw new ArgumentNullException("waitHandle");
        }

        return FromWaitHandleInternal(waitHandle);
    }

    public static Task FromWaitHandle(WaitHandle waitHandle, CancellationToken cancellationToken)
    {
        if (waitHandle == null)
        {
            throw new ArgumentNullException("waitHandle");
        }

        return FromWaitHandleInternal(waitHandle, cancellationToken);
    }

    public static Task<bool> FromWaitHandle(WaitHandle waitHandle, int millisecondsTimeout)
    {
        if (waitHandle == null)
        {
            throw new ArgumentNullException("waitHandle");
        }

        return FromWaitHandleInternal(waitHandle, millisecondsTimeout);
    }

    public static Task<bool> FromWaitHandle(WaitHandle waitHandle, int millisecondsTimeout,
        CancellationToken cancellationToken)
    {
        if (waitHandle == null)
        {
            throw new ArgumentNullException("waitHandle");
        }

        return FromWaitHandleInternal(waitHandle, millisecondsTimeout, cancellationToken);
    }

    public static Task<bool> FromWaitHandle(WaitHandle waitHandle, TimeSpan timeout)
    {
        if (waitHandle == null)
        {
            throw new ArgumentNullException("waitHandle");
        }

        return FromWaitHandleInternal(waitHandle, (int)timeout.TotalMilliseconds);
    }

    public static Task<bool> FromWaitHandle(WaitHandle waitHandle, TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        if (waitHandle == null)
        {
            throw new ArgumentNullException("waitHandle");
        }

        return FromWaitHandleInternal(waitHandle, (int)timeout.TotalMilliseconds, cancellationToken);
    }

    internal static Task FromWaitHandleInternal(WaitHandle waitHandle)
    {
        TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
        if (waitHandle.WaitOne(0, exitContext: false))
        {
            taskCompletionSource.SetResult(result: true);
        }

        WaitHandleTaskCompletionSourceManager.CreateWithoutTimeout(waitHandle, taskCompletionSource);
        return taskCompletionSource.Task;
    }

    internal static Task FromWaitHandleInternal(WaitHandle waitHandle, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return FromCanceled<bool>(cancellationToken);
        }

        TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
        if (waitHandle.WaitOne(0, exitContext: false))
        {
            taskCompletionSource.SetResult(result: true);
        }

        WaitHandleCancellableTaskCompletionSourceManager.CreateWithoutTimeout(waitHandle, cancellationToken,
            taskCompletionSource);
        return taskCompletionSource.Task;
    }

    internal static Task<bool> FromWaitHandleInternal(WaitHandle waitHandle, int millisecondsTimeout)
    {
        if (millisecondsTimeout < -1)
        {
            throw new ArgumentOutOfRangeException("millisecondsTimeout");
        }

        TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
        if (waitHandle.WaitOne(0, exitContext: false))
        {
            taskCompletionSource.SetResult(result: true);
        }
        else if (millisecondsTimeout == -1)
        {
            WaitHandleTaskCompletionSourceManager.CreateWithoutTimeout(waitHandle, taskCompletionSource);
        }
        else
        {
            WaitHandleTaskCompletionSourceManager.CreateWithTimeout(waitHandle, taskCompletionSource,
                millisecondsTimeout);
        }

        return taskCompletionSource.Task;
    }

    internal static Task<bool> FromWaitHandleInternal(WaitHandle waitHandle, int millisecondsTimeout,
        CancellationToken cancellationToken)
    {
        if (millisecondsTimeout < -1)
        {
            throw new ArgumentOutOfRangeException("millisecondsTimeout");
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return FromCanceled<bool>(cancellationToken);
        }

        TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
        if (waitHandle.WaitOne(0, exitContext: false))
        {
            taskCompletionSource.SetResult(result: true);
        }
        else if (millisecondsTimeout == -1)
        {
            WaitHandleCancellableTaskCompletionSourceManager.CreateWithoutTimeout(waitHandle, cancellationToken,
                taskCompletionSource);
        }
        else
        {
            WaitHandleCancellableTaskCompletionSourceManager.CreateWithTimeout(waitHandle, cancellationToken,
                taskCompletionSource, millisecondsTimeout);
        }

        return taskCompletionSource.Task;
    }

    internal static Task FromWaitHandleInternal(WaitHandle waitHandle, TaskCreationOptions creationOptions)
    {
        TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>(creationOptions);
        if (waitHandle.WaitOne(0, exitContext: false))
        {
            taskCompletionSource.SetResult(result: true);
        }

        WaitHandleTaskCompletionSourceManager.CreateWithoutTimeout(waitHandle, taskCompletionSource);
        return taskCompletionSource.Task;
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task FromCanceled(CancellationToken cancellationToken)
    {
        return FromCanceled<VoidStruct>(cancellationToken);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task<TResult> FromCanceled<TResult>(CancellationToken cancellationToken)
    {
        if (!cancellationToken.IsCancellationRequested)
        {
            throw new ArgumentOutOfRangeException("cancellationToken");
        }

        TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
        TaskCompletionSourceTheraotExtensions.TrySetCanceled(taskCompletionSource, cancellationToken);
        return taskCompletionSource.Task;
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task<TResult> FromException<TResult>(Exception exception)
    {
        if (exception == null)
        {
            throw new ArgumentNullException("exception");
        }

        TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
        taskCompletionSource.TrySetException(exception);
        return taskCompletionSource.Task;
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task FromException(Exception exception)
    {
        return FromException<VoidStruct>(exception);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task<TResult> FromResult<TResult>(TResult result)
    {
        TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
        taskCompletionSource.TrySetResult(result);
        return taskCompletionSource.Task;
    }
}