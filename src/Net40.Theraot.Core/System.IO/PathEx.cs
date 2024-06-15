namespace System.IO;

public static class PathEx
{
	public static bool EndsInDirectorySeparator(string path)
	{
		if (path != null && path.Length > 0)
		{
			return PathInternalEx.IsDirectorySeparator(path[path.Length - 1]);
		}
		return false;
	}
}