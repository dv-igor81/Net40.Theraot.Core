using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks.Sources;

namespace System.Threading.Tasks;

[StructLayout(LayoutKind.Auto)]
[AsyncMethodBuilder(typeof(AsyncValueTaskMethodBuilder))]
public readonly struct ValueTask : IEquatable<ValueTask>
{
    private sealed class ValueTaskSourceAsTask : TaskCompletionSource<bool>
    {
        private static readonly Action<object> s_completionAction = delegate(object state)
        {
            IValueTaskSource source;
            if (!(state is ValueTaskSourceAsTask valueTaskSourceAsTask) ||
                (source = valueTaskSourceAsTask._source) == null)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.state);
                return;
            }

            valueTaskSourceAsTask._source = null;
            ValueTaskSourceStatus status = source.GetStatus(valueTaskSourceAsTask._token);
            try
            {
                source.GetResult(valueTaskSourceAsTask._token);
                valueTaskSourceAsTask.TrySetResult(result: false);
            }
            catch (Exception exception)
            {
                if (status == ValueTaskSourceStatus.Canceled)
                {
                    valueTaskSourceAsTask.TrySetCanceled();
                }
                else
                {
                    valueTaskSourceAsTask.TrySetException(exception);
                }
            }
        };

        private IValueTaskSource _source;

        private readonly short _token;

        public ValueTaskSourceAsTask(IValueTaskSource source, short token)
        {
            _token = token;
            _source = source;
            source.OnCompleted(s_completionAction, this, token, ValueTaskSourceOnCompletedFlags.None);
        }
    }

    private static readonly Task s_canceledTask = TaskEx.Delay(-1, new CancellationToken(canceled: true));

    internal readonly object _obj;

    internal readonly short _token;

    internal readonly bool _continueOnCapturedContext;

    public static Task CompletedTask { get; } = TaskEx.Delay(0);


    public bool IsCompleted
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        get
        {
            object obj = _obj;
            if (obj == null)
            {
                return true;
            }

            if (!(obj is Task { IsCompleted: var isCompleted }))
            {
                return ((IValueTaskSource)obj).GetStatus(_token) != ValueTaskSourceStatus.Pending;
            }

            return isCompleted;
        }
    }

    public bool IsCompletedSuccessfully
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        get
        {
            object obj = _obj;
            if (obj == null)
            {
                return true;
            }

            if (obj is Task task)
            {
                return task.Status == TaskStatus.RanToCompletion;
            }

            return ((IValueTaskSource)obj).GetStatus(_token) == ValueTaskSourceStatus.Succeeded;
        }
    }

    public bool IsFaulted
    {
        get
        {
            object obj = _obj;
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Task { IsFaulted: var isFaulted }))
            {
                return ((IValueTaskSource)obj).GetStatus(_token) == ValueTaskSourceStatus.Faulted;
            }

            return isFaulted;
        }
    }

    public bool IsCanceled
    {
        get
        {
            object obj = _obj;
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Task { IsCanceled: var isCanceled }))
            {
                return ((IValueTaskSource)obj).GetStatus(_token) == ValueTaskSourceStatus.Canceled;
            }

            return isCanceled;
        }
    }


    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public ValueTask(Task task)
    {
        if (task == null)
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.task);
        }

        _obj = task;
        _continueOnCapturedContext = true;
        _token = 0;
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static ValueTask FromCanceled(CancellationToken cancellationToken)
    {
        return new ValueTask(TaskExEx.FromCanceled(cancellationToken));
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static ValueTask<TResult> FromCanceled<TResult>(CancellationToken cancellationToken)
    {
        return new ValueTask<TResult>(TaskExEx.FromCanceled<TResult>(cancellationToken));
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static ValueTask FromException(Exception exception)
    {
        return new ValueTask(TaskExEx.FromException(exception));
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static ValueTask<TResult> FromException<TResult>(Exception exception)
    {
        return new ValueTask<TResult>(TaskExEx.FromException<TResult>(exception));
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public ValueTask(IValueTaskSource source, short token)
    {
        if (source == null)
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
        }

        _obj = source;
        _token = token;
        _continueOnCapturedContext = true;
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    private ValueTask(object obj, short token, bool continueOnCapturedContext)
    {
        _obj = obj;
        _token = token;
        _continueOnCapturedContext = continueOnCapturedContext;
    }

    public override int GetHashCode()
    {
        return _obj?.GetHashCode() ?? 0;
    }

    public override bool Equals(object obj)
    {
        if (obj is ValueTask)
        {
            return Equals((ValueTask)obj);
        }

        return false;
    }

    public bool Equals(ValueTask other)
    {
        if (_obj == other._obj)
        {
            return _token == other._token;
        }

        return false;
    }

    public static bool operator ==(ValueTask left, ValueTask right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ValueTask left, ValueTask right)
    {
        return !left.Equals(right);
    }

    public Task AsTask()
    {
        object obj = _obj;
        object obj2;
        if (obj != null)
        {
            obj2 = obj as Task;
            if (obj2 == null)
            {
                return GetTaskForValueTaskSource((IValueTaskSource)obj);
            }
        }
        else
        {
            obj2 = CompletedTask;
        }

        return (Task)obj2;
    }

    public ValueTask Preserve()
    {
        if (_obj != null)
        {
            return new ValueTask(AsTask());
        }

        return this;
    }

    private Task GetTaskForValueTaskSource(IValueTaskSource t)
    {
        ValueTaskSourceStatus status = t.GetStatus(_token);
        if (status != 0)
        {
            try
            {
                t.GetResult(_token);
                return CompletedTask;
            }
            catch (Exception exception)
            {
                if (status == ValueTaskSourceStatus.Canceled)
                {
                    return s_canceledTask;
                }

                TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
                taskCompletionSource.TrySetException(exception);
                return taskCompletionSource.Task;
            }
        }

        ValueTaskSourceAsTask valueTaskSourceAsTask = new ValueTaskSourceAsTask(t, _token);
        return valueTaskSourceAsTask.Task;
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    [StackTraceHidden]
    internal void ThrowIfCompletedUnsuccessfully()
    {
        object obj = _obj;
        if (obj != null)
        {
            if (obj is Task task)
            {
                TaskTheraotExtensions.GetAwaiter(task).GetResult();
            }
            else
            {
                ((IValueTaskSource)obj).GetResult(_token);
            }
        }
    }

    public ValueTaskAwaiter GetAwaiter()
    {
        return new ValueTaskAwaiter(this);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public ConfiguredValueTaskAwaitable ConfigureAwait(bool continueOnCapturedContext)
    {
        return new ConfiguredValueTaskAwaitable(new ValueTask(_obj, _token, continueOnCapturedContext));
    }
}

[StructLayout(LayoutKind.Auto)]
[AsyncMethodBuilder(typeof(AsyncValueTaskMethodBuilder<>))]
public readonly struct ValueTask<TResult> : IEquatable<ValueTask<TResult>>
{
    private sealed class ValueTaskSourceAsTask : TaskCompletionSource<TResult>
    {
        private static readonly Action<object> s_completionAction = delegate(object state)
        {
            IValueTaskSource<TResult> source;
            if (!(state is ValueTaskSourceAsTask valueTaskSourceAsTask) ||
                (source = valueTaskSourceAsTask._source) == null)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.state);
                return;
            }

            valueTaskSourceAsTask._source = null;
            ValueTaskSourceStatus status = source.GetStatus(valueTaskSourceAsTask._token);
            try
            {
                valueTaskSourceAsTask.TrySetResult(source.GetResult(valueTaskSourceAsTask._token));
            }
            catch (Exception exception)
            {
                if (status == ValueTaskSourceStatus.Canceled)
                {
                    valueTaskSourceAsTask.TrySetCanceled();
                }
                else
                {
                    valueTaskSourceAsTask.TrySetException(exception);
                }
            }
        };

        private IValueTaskSource<TResult> _source;

        private readonly short _token;

        public ValueTaskSourceAsTask(IValueTaskSource<TResult> source, short token)
        {
            _source = source;
            _token = token;
            source.OnCompleted(s_completionAction, this, token, ValueTaskSourceOnCompletedFlags.None);
        }
    }

    private static Task<TResult> s_canceledTask;

    internal readonly object _obj;

    internal readonly TResult _result;

    internal readonly short _token;

    internal readonly bool _continueOnCapturedContext;

    public bool IsCompleted
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        get
        {
            object obj = _obj;
            if (obj == null)
            {
                return true;
            }

            if (!(obj is Task<TResult> { IsCompleted: var isCompleted }))
            {
                return ((IValueTaskSource<TResult>)obj).GetStatus(_token) != ValueTaskSourceStatus.Pending;
            }

            return isCompleted;
        }
    }

    public bool IsCompletedSuccessfully
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        get
        {
            object obj = _obj;
            if (obj == null)
            {
                return true;
            }

            if (obj is Task<TResult> task)
            {
                return task.Status == TaskStatus.RanToCompletion;
            }

            return ((IValueTaskSource<TResult>)obj).GetStatus(_token) == ValueTaskSourceStatus.Succeeded;
        }
    }

    public bool IsFaulted
    {
        get
        {
            object obj = _obj;
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Task<TResult> { IsFaulted: var isFaulted }))
            {
                return ((IValueTaskSource<TResult>)obj).GetStatus(_token) == ValueTaskSourceStatus.Faulted;
            }

            return isFaulted;
        }
    }

    public bool IsCanceled
    {
        get
        {
            object obj = _obj;
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Task<TResult> { IsCanceled: var isCanceled }))
            {
                return ((IValueTaskSource<TResult>)obj).GetStatus(_token) == ValueTaskSourceStatus.Canceled;
            }

            return isCanceled;
        }
    }

    public TResult Result
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        get
        {
            object obj = _obj;
            if (obj == null)
            {
                return _result;
            }

            if (obj is Task<TResult> task)
            {
                return TaskTheraotExtensions.GetAwaiter(task).GetResult();
            }

            return ((IValueTaskSource<TResult>)obj).GetResult(_token);
        }
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public ValueTask(TResult result)
    {
        _result = result;
        _obj = null;
        _continueOnCapturedContext = true;
        _token = 0;
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public ValueTask(Task<TResult> task)
    {
        if (task == null)
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.task);
        }

        _obj = task;
        _result = default(TResult);
        _continueOnCapturedContext = true;
        _token = 0;
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public ValueTask(IValueTaskSource<TResult> source, short token)
    {
        if (source == null)
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
        }

        _obj = source;
        _token = token;
        _result = default(TResult);
        _continueOnCapturedContext = true;
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    private ValueTask(object obj, TResult result, short token, bool continueOnCapturedContext)
    {
        _obj = obj;
        _result = result;
        _token = token;
        _continueOnCapturedContext = continueOnCapturedContext;
    }

    public override int GetHashCode()
    {
        if (_obj == null)
        {
            if (_result == null)
            {
                return 0;
            }

            return _result.GetHashCode();
        }

        return _obj.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj is ValueTask<TResult>)
        {
            return Equals((ValueTask<TResult>)obj);
        }

        return false;
    }

    public bool Equals(ValueTask<TResult> other)
    {
        if (_obj == null && other._obj == null)
        {
            return EqualityComparer<TResult>.Default.Equals(_result, other._result);
        }

        if (_obj == other._obj)
        {
            return _token == other._token;
        }

        return false;
    }

    public static bool operator ==(ValueTask<TResult> left, ValueTask<TResult> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ValueTask<TResult> left, ValueTask<TResult> right)
    {
        return !left.Equals(right);
    }

    public Task<TResult> AsTask()
    {
        object obj = _obj;
        if (obj == null)
        {
            return TaskEx.FromResult(_result);
        }

        if (obj is Task<TResult> result)
        {
            return result;
        }

        return GetTaskForValueTaskSource((IValueTaskSource<TResult>)obj);
    }

    public ValueTask<TResult> Preserve()
    {
        if (_obj != null)
        {
            return new ValueTask<TResult>(AsTask());
        }

        return this;
    }

    private Task<TResult> GetTaskForValueTaskSource(IValueTaskSource<TResult> t)
    {
        ValueTaskSourceStatus status = t.GetStatus(_token);
        if (status != 0)
        {
            try
            {
                return TaskEx.FromResult(t.GetResult(_token));
            }
            catch (Exception exception)
            {
                if (status == ValueTaskSourceStatus.Canceled)
                {
                    Task<TResult> task = s_canceledTask;
                    if (task == null)
                    {
                        TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
                        taskCompletionSource.TrySetCanceled();
                        task = (s_canceledTask = taskCompletionSource.Task);
                    }

                    return task;
                }

                TaskCompletionSource<TResult> taskCompletionSource2 = new TaskCompletionSource<TResult>();
                taskCompletionSource2.TrySetException(exception);
                return taskCompletionSource2.Task;
            }
        }

        ValueTaskSourceAsTask valueTaskSourceAsTask = new ValueTaskSourceAsTask(t, _token);
        return valueTaskSourceAsTask.Task;
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public ValueTaskAwaiter<TResult> GetAwaiter()
    {
        return new ValueTaskAwaiter<TResult>(this);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public ConfiguredValueTaskAwaitable<TResult> ConfigureAwait(bool continueOnCapturedContext)
    {
        return new ConfiguredValueTaskAwaitable<TResult>(new ValueTask<TResult>(_obj, _result, _token,
            continueOnCapturedContext));
    }

    public override string ToString()
    {
        if (IsCompletedSuccessfully)
        {
            TResult result = Result;
            if (result != null)
            {
                return result.ToString();
            }
        }

        return string.Empty;
    }
}