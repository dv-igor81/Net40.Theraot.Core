using System.Collections.Generic;

namespace Theraot.Collections.Specialized;

public interface IHasComparer<in TKey>
{
	IEqualityComparer<TKey> Comparer { get; }
}