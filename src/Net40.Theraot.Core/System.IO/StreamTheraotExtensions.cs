using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Theraot.Collections.ThreadSafe;

namespace System.IO;

public static class StreamTheraotExtensions
{
    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task CopyToAsync(this Stream source, Stream destination)
    {
        if (source == null)
        {
            throw new NullReferenceException();
        }

        if (destination == null)
        {
            throw new ArgumentNullException("destination");
        }

        if (!source.CanRead)
        {
            throw new NotSupportedException("Source stream does not support read.");
        }

        if (!destination.CanWrite)
        {
            throw new NotSupportedException("Destination stream does not support write.");
        }

        return CopyToPrivateAsync(source, destination, 65536, CancellationToken.None);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task CopyToAsync(this Stream source, Stream destination, int bufferSize)
    {
        if (source == null)
        {
            throw new NullReferenceException();
        }

        if (destination == null)
        {
            throw new ArgumentNullException("destination");
        }

        if (bufferSize <= 0)
        {
            throw new ArgumentOutOfRangeException("bufferSize");
        }

        if (!source.CanRead)
        {
            throw new NotSupportedException("Source stream does not support read.");
        }

        if (!destination.CanWrite)
        {
            throw new NotSupportedException("Destination stream does not support write.");
        }

        return CopyToPrivateAsync(source, destination, bufferSize, CancellationToken.None);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task CopyToAsync(this Stream source, Stream destination, CancellationToken cancellationToken)
    {
        if (source == null)
        {
            throw new NullReferenceException();
        }

        if (destination == null)
        {
            throw new ArgumentNullException("destination");
        }

        if (!source.CanRead)
        {
            throw new NotSupportedException("Source stream does not support read.");
        }

        if (!destination.CanWrite)
        {
            throw new NotSupportedException("Destination stream does not support write.");
        }

        return CopyToPrivateAsync(source, destination, 65536, cancellationToken);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task CopyToAsync(this Stream source, Stream destination, int bufferSize,
        CancellationToken cancellationToken)
    {
        if (source == null)
        {
            throw new NullReferenceException();
        }

        if (destination == null)
        {
            throw new ArgumentNullException("destination");
        }

        if (bufferSize <= 0)
        {
            throw new ArgumentOutOfRangeException("bufferSize");
        }

        if (!source.CanRead)
        {
            throw new NotSupportedException("Source stream does not support read.");
        }

        if (!destination.CanWrite)
        {
            throw new NotSupportedException("Destination stream does not support write.");
        }

        return CopyToPrivateAsync(source, destination, bufferSize, cancellationToken);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task FlushAsync(this Stream stream)
    {
        if (stream == null)
        {
            throw new NullReferenceException();
        }

        return TaskEx.Run((Action)stream.Flush);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task FlushAsync(this Stream stream, CancellationToken token)
    {
        if (stream == null)
        {
            throw new NullReferenceException();
        }

        token.ThrowIfCancellationRequested();
        return TaskEx.Run((Action)stream.Flush, token);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task<int> ReadAsync(this Stream stream, byte[] buffer, int offset, int count,
        CancellationToken cancellationToken)
    {
        if (stream == null)
        {
            throw new NullReferenceException();
        }

        cancellationToken.ThrowIfCancellationRequested();
        return Task.Factory.FromAsync((Func<byte[], int, int, AsyncCallback, object, IAsyncResult>)BeginRead,
            (Func<IAsyncResult, int>)stream.EndRead, buffer, offset, count, (object)stream);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task<int> ReadAsync(this Stream stream, byte[] buffer, int offset, int count)
    {
        if (stream == null)
        {
            throw new NullReferenceException();
        }

        return Task.Factory.FromAsync((Func<byte[], int, int, AsyncCallback, object, IAsyncResult>)BeginRead,
            (Func<IAsyncResult, int>)stream.EndRead, buffer, offset, count, (object)stream);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task WriteAsync(this Stream stream, byte[] buffer, int offset, int count,
        CancellationToken cancellationToken)
    {
        if (stream == null)
        {
            throw new NullReferenceException();
        }

        cancellationToken.ThrowIfCancellationRequested();
        return Task.Factory.FromAsync(BeginWrite, stream.EndWrite, buffer, offset, count, stream);
    }

    [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
    public static Task WriteAsync(this Stream stream, byte[] buffer, int offset, int count)
    {
        if (stream == null)
        {
            throw new NullReferenceException();
        }

        return Task.Factory.FromAsync(BeginWrite, stream.EndWrite, buffer, offset, count, stream);
    }

    private static IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
    {
        Stream stream = (Stream)state;
        return stream.BeginRead(buffer, offset, count, callback, count);
    }

    private static IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
    {
        Stream stream = (Stream)state;
        return stream.BeginWrite(buffer, offset, count, callback, count);
    }

    private static async Task CopyToPrivateAsync(Stream source, Stream destination, int bufferSize,
        CancellationToken cancellationToken)
    {
        byte[] buffer = ArrayReservoir<byte>.GetArray(bufferSize);
        try
        {
            while (true)
            {
                int bytesRead = await TaskTheraotExtensions.ConfigureAwait(
                    ReadAsync(source, buffer, 0, bufferSize, cancellationToken), continueOnCapturedContext: false);
                if (bytesRead == 0)
                {
                    break;
                }

                await TaskTheraotExtensions.ConfigureAwait(
                    WriteAsync(destination, buffer, 0, bytesRead, cancellationToken), continueOnCapturedContext: false);
            }
        }
        finally
        {
            ArrayReservoir<byte>.DonateArray(buffer);
        }
    }

    public static ValueTask DisposeAsync(this Stream stream)
    {
        try
        {
            stream.Dispose();
            return default(ValueTask);
        }
        catch (Exception exception)
        {
            return ValueTask.FromException(exception);
        }
    }
}