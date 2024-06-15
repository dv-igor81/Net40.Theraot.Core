using System;

namespace Theraot.Threading;

public readonly struct UniqueId : IEquatable<UniqueId>
{
    private readonly uint _id;

    internal UniqueId(uint id)
    {
            _id = id;
        }

    public static bool operator !=(UniqueId x, UniqueId y)
    {
            return !x.Equals(y);
        }

    public static bool operator ==(UniqueId x, UniqueId y)
    {
            return x.Equals(y);
        }

    public override bool Equals(object obj)
    {
            if (obj is UniqueId other)
            {
                return Equals(other);
            }

            return false;
        }

    public bool Equals(UniqueId other)
    {
            return other._id == _id;
        }

    public override int GetHashCode()
    {
            return (int)_id;
        }

    public override string ToString()
    {
            return $"{_id}";
        }
}