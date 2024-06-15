// Theraot.Core, Version=3.2.11.0, Culture=neutral, PublicKeyToken=b1460dff8a28f7a7
// System.Threading.Tasks.ValueTaskEx
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

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
    
    public static ValueTask CompletedTask => new(ValueTask.CompletedTask);
}