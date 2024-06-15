using System;
using System.Diagnostics;

namespace Theraot.Threading.Needles;

[DebuggerNonUserCode]
public class ReadOnlyPromiseNeedle<T> : ReadOnlyPromise, ICacheNeedle<T>, INeedle<T>, IReadOnlyNeedle<T>,
    IPromise<T>, IPromise, IEquatable<ReadOnlyPromiseNeedle<T>>
{
    private readonly ICacheNeedle<T> _promised;

    public bool IsAlive => _promised.IsAlive;

    public T Value => _promised.Value;

    T INeedle<T>.Value
    {
        get { return _promised.Value; }
        set { throw new NotSupportedException(); }
    }

    public ReadOnlyPromiseNeedle(PromiseNeedle<T> promised)
        : base(promised)
    {
            _promised = promised;
        }

    public static bool operator !=(ReadOnlyPromiseNeedle<T> left, ReadOnlyPromiseNeedle<T> right)
    {
            if ((object)left == null)
            {
                return (object)right != null;
            }

            if ((object)right == null)
            {
                return true;
            }

            return !left._promised.Equals(right._promised);
        }

    public static bool operator ==(ReadOnlyPromiseNeedle<T> left, ReadOnlyPromiseNeedle<T> right)
    {
            if ((object)left == null)
            {
                return (object)right == null;
            }

            return (object)right != null && left._promised.Equals(right._promised);
        }

    public override bool Equals(object obj)
    {
            if (obj is ReadOnlyPromiseNeedle<T> readOnlyPromiseNeedle)
            {
                return _promised.Equals(readOnlyPromiseNeedle._promised);
            }

            if (!_promised.IsCompleted)
            {
                return false;
            }

            T value;
            bool flag = _promised.TryGetValue(out value);
            if (obj == null)
            {
                return !flag;
            }

            return flag && obj.Equals(value);
        }

    public bool Equals(ReadOnlyPromiseNeedle<T> other)
    {
            return (object)other != null && _promised.Equals(other._promised);
        }

    public override int GetHashCode()
    {
            return _promised.GetHashCode();
        }

    public override string ToString()
    {
            return $"{{Promise: {_promised}}}";
        }

    public bool TryGetValue(out T value)
    {
            return _promised.TryGetValue(out value);
        }
}