#define TRACE
using System.Diagnostics;
using System.Reflection;

namespace System.Threading.Tasks;

public static class TaskCompletionSourceTheraotExtensions
{
    private static class TrySetCanceledCachedDelegate<T>
    {
        private static Func<TaskCompletionSource<T>, CancellationToken, bool>? _trySetCanceledCached;

        public static Func<TaskCompletionSource<T>, CancellationToken, bool> TrySetCanceledCached =>
            _trySetCanceledCached ?? (_trySetCanceledCached = CreateTrySetCanceledDelegate());

        private static Func<TaskCompletionSource<T>, CancellationToken, bool> CreateTrySetCanceledDelegate()
        {
            MethodInfo method =
                typeof(TaskCompletionSource<T>).GetMethod("TrySetCanceled", new Type[1] { typeof(CancellationToken) });
            if (method != null)
            {
                return (Func<TaskCompletionSource<T>, CancellationToken, bool>)MethodInfoTheraotExtensions
                    .CreateDelegate(method, typeof(Func<TaskCompletionSource<T>, CancellationToken, bool>));
            }

            new TraceSource("Theraot.Core").TraceEvent(TraceEventType.Warning, 1,
                "TaskCompletionSource<T>.TrySetCanceled(CancellationToken): fallback to overload without CancellationToken.");
            method = typeof(TaskCompletionSource<T>).GetMethod("TrySetCanceled");
            if (method == null)
            {
                throw new PlatformNotSupportedException("Method not found: TaskCompletionSource.TrySetCanceled");
            }

            Func<TaskCompletionSource<T>, bool> trySetCanceled =
                (Func<TaskCompletionSource<T>, bool>)MethodInfoTheraotExtensions.CreateDelegate(method,
                    typeof(Func<TaskCompletionSource<T>, bool>));
            return (TaskCompletionSource<T> tcs, CancellationToken ct) => trySetCanceled(tcs);
        }
    }

    public static bool TrySetCanceled<T>(this TaskCompletionSource<T> taskCompletionSource,
        CancellationToken cancellationToken)
    {
        if (taskCompletionSource == null)
        {
            throw new ArgumentNullException("taskCompletionSource");
        }

        return TrySetCanceledCachedDelegate<T>.TrySetCanceledCached(taskCompletionSource, cancellationToken);
    }

    public static void SetCanceled<T>(this TaskCompletionSource<T> taskCompletionSource,
        CancellationToken cancellationToken)
    {
        if (taskCompletionSource == null)
        {
            throw new ArgumentNullException("taskCompletionSource");
        }

        if (!TrySetCanceled(taskCompletionSource, cancellationToken))
        {
            throw new InvalidOperationException(
                "An attempt was made to transition a task to a final state when it had already completed.");
        }
    }
}