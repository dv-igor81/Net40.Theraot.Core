namespace Theraot.Threading.Needles;

public static class NeedleHelper
{
	public static bool TryGetValue<T>(this IReadOnlyNeedle<T> needle, out T target)
	{
		if (needle == null)
		{
			target = default(T);
			return false;
		}
		if (needle is ICacheNeedle<T> cacheNeedle)
		{
			return cacheNeedle.TryGetValue(out target);
		}
		target = needle.Value;
		return needle.IsAlive;
	}
}