using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Threading.Needles;

[DebuggerNonUserCode]
public struct StructNeedle<T> : INeedle<T>, IReadOnlyNeedle<T>, IEquatable<StructNeedle<T>>, IRecyclable
{
    public bool IsAlive => Value != null;

    public T Value { get; set; }

    public StructNeedle(T target)
    {
            Value = target;
        }

    public static bool operator !=(StructNeedle<T> left, StructNeedle<T> right)
    {
            T value = left.Value;
            if (!left.IsAlive)
            {
                return right.IsAlive;
            }

            T value2 = right.Value;
            return !right.IsAlive || !EqualityComparer<T>.Default.Equals(value, value2);
        }

    public static bool operator ==(StructNeedle<T> left, StructNeedle<T> right)
    {
            T value = left.Value;
            if (!left.IsAlive)
            {
                return !right.IsAlive;
            }

            T value2 = right.Value;
            return right.IsAlive && EqualityComparer<T>.Default.Equals(value, value2);
        }

    public override bool Equals(object obj)
    {
            if (!(obj is StructNeedle<T> other))
            {
                if (obj is T otherValue)
                {
                    return Equals(otherValue);
                }

                return false;
            }

            return Equals(other);
        }

    public bool Equals(StructNeedle<T> other)
    {
            T value = other.Value;
            if (other.IsAlive)
            {
                return Equals(value);
            }

            return !IsAlive;
        }

    void IRecyclable.Free()
    {
            Value = default(T);
        }

    public override int GetHashCode()
    {
            try
            {
                return EqualityComparer<T>.Default.GetHashCode(Value);
            }
            catch (ArgumentNullException)
            {
                return 0;
            }
        }

    public override string ToString()
    {
            T value = Value;
            object obj;
            if (!IsAlive)
            {
                obj = "<Dead Needle>";
            }
            else
            {
                obj = value?.ToString();
                if (obj == null)
                {
                    obj = "<?>";
                }
            }

            return (string)obj;
        }

    private bool Equals(T otherValue)
    {
            T value = Value;
            return IsAlive && EqualityComparer<T>.Default.Equals(value, otherValue);
        }
}