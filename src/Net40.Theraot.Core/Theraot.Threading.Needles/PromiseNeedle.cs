using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Threading.Needles;

[DebuggerNonUserCode]
public class PromiseNeedle<T> : Promise, ICacheNeedle<T>, INeedle<T>, IReadOnlyNeedle<T>, IPromise<T>, IPromise,
    IEquatable<PromiseNeedle<T>>
{
    private readonly int _hashCode;

    private T _target;

    public bool IsAlive => _target != null;

    public virtual T Value
    {
        get
        {
                Exception exception = Exception;
                if (exception == null)
                {
                    return _target;
                }

                throw exception;
            }
        set
        {
                _target = value;
                SetCompleted();
            }
    }

    public PromiseNeedle(bool done)
        : base(done)
    {
            _target = default(T);
            _hashCode = base.GetHashCode();
        }

    public PromiseNeedle(Exception exception)
        : base(exception)
    {
            _target = default(T);
            _hashCode = exception.GetHashCode();
        }

    protected PromiseNeedle(T target)
        : base(done: true)
    {
            _target = target;
            _hashCode = target?.GetHashCode() ?? base.GetHashCode();
        }

    public static PromiseNeedle<T> CreateFromValue(T target)
    {
            return new PromiseNeedle<T>(target);
        }

    public static bool operator !=(PromiseNeedle<T> left, PromiseNeedle<T> right)
    {
            if ((object)left == null)
            {
                return (object)right != null;
            }

            if ((object)right == null)
            {
                return true;
            }

            return !EqualityComparer<T>.Default.Equals(left._target, right._target);
        }

    public static bool operator ==(PromiseNeedle<T> left, PromiseNeedle<T> right)
    {
            return ((object)left == null)
                ? ((object)right == null)
                : ((object)right != null && EqualityComparer<T>.Default.Equals(left._target, right._target));
        }

    public override bool Equals(object obj)
    {
            if (!(obj is PromiseNeedle<T> other))
            {
                if (obj is T otherValue)
                {
                    return Equals(otherValue);
                }

                return false;
            }

            return Equals(other);
        }

    public bool Equals(PromiseNeedle<T> other)
    {
            if ((object)other == null)
            {
                return false;
            }

            if (other.TryGetValue(out var value))
            {
                return Equals(value);
            }

            return !IsAlive;
        }

    public override void Free()
    {
            base.Free();
            _target = default(T);
        }

    public override int GetHashCode()
    {
            return _hashCode;
        }

    public override string ToString()
    {
            T target = _target;
            if (!IsCompleted)
            {
                return "[Not Created]";
            }

            if (Exception == null)
            {
                object obj = target?.ToString();
                if (obj == null)
                {
                    obj = "[?]";
                }

                return (string)obj;
            }

            return Exception.ToString() ?? "[?]";
        }

    public bool TryGetValue(out T value)
    {
            bool isCompleted = IsCompleted;
            value = _target;
            return isCompleted;
        }

    private bool Equals(T otherValue)
    {
            T value = Value;
            return IsAlive && EqualityComparer<T>.Default.Equals(value, otherValue);
        }
}