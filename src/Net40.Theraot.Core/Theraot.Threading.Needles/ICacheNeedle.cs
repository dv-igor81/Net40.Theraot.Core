namespace Theraot.Threading.Needles;

public interface ICacheNeedle<T> : INeedle<T>, IReadOnlyNeedle<T>, IPromise<T>, IPromise
{
	bool TryGetValue(out T value);
}
