using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace Theraot.Threading.Needles;

[DebuggerNonUserCode]
public readonly struct ExceptionStructNeedle<T> : INeedle<T>, IReadOnlyNeedle<T>,
    IEquatable<ExceptionStructNeedle<T>>
{
    private readonly ExceptionDispatchInfo _exceptionDispatchInfo;

    public Exception Exception => _exceptionDispatchInfo.SourceException;

    public bool IsAlive => false;

    public T Value
    {
        get
        {
                _exceptionDispatchInfo.Throw();
                return default(T);
            }
    }

    T INeedle<T>.Value
    {
        get { return Value; }
        set { throw new NotSupportedException(); }
    }

    public ExceptionStructNeedle(Exception exception)
    {
            _exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
        }

    public static bool operator !=(ExceptionStructNeedle<T> left, ExceptionStructNeedle<T> right)
    {
            Exception exception = left.Exception;
            Exception exception2 = right.Exception;
            if (exception == null)
            {
                return exception2 != null;
            }

            return !exception.Equals(exception2);
        }

    public static bool operator ==(ExceptionStructNeedle<T> left, ExceptionStructNeedle<T> right)
    {
            Exception exception = left.Exception;
            Exception exception2 = right.Exception;
            return exception?.Equals(exception2) ?? (exception2 == null);
        }

    public bool Equals(ExceptionStructNeedle<T> other)
    {
            return this == other;
        }

    public override bool Equals(object obj)
    {
            if (!(obj is ExceptionStructNeedle<T> exceptionStructNeedle))
            {
                if (obj is ExceptionDispatchInfo exceptionDispatchInfo)
                {
                    return exceptionDispatchInfo.Equals(_exceptionDispatchInfo);
                }

                return obj is Exception ex && ex.Equals(Exception);
            }

            return this == exceptionStructNeedle;
        }

    public override int GetHashCode()
    {
            return EqualityComparer<ExceptionDispatchInfo>.Default.GetHashCode(_exceptionDispatchInfo);
        }

    public override string ToString()
    {
            return IsAlive ? $"<Exception: {Exception}>" : "<Dead Needle>";
        }
}