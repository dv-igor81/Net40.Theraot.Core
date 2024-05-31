using System.Runtime.CompilerServices;

namespace System.Threading.Tasks;

public static class ValueTaskEx
{
    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static ValueTask FromCanceled(CancellationToken cancellationToken)
    {
        return ValueTask.FromCanceled(cancellationToken);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static ValueTask<TResult> FromCanceled<TResult>(CancellationToken cancellationToken)
    {
        return ValueTask.FromCanceled<TResult>(cancellationToken);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static ValueTask FromException(Exception exception)
    {
        return ValueTask.FromException(exception);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static ValueTask<TResult> FromException<TResult>(Exception exception)
    {
        return ValueTask.FromException<TResult>(exception);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static ValueTask<TResult> FromResult<TResult>(TResult result)
    {
        return new ValueTask<TResult>(result);
    }
    
    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static ValueTask<TResult> FromTask<TResult>(Task<TResult> task)
    {
        return new ValueTask<TResult>(task);
    }
    
    public static ValueTask CompletedTask => new(ValueTask.CompletedTask);
}
