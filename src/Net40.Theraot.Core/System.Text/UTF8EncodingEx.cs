namespace System.Text;

public static class UTF8EncodingEx
{
	public static string GetString(this UTF8Encoding self, byte[] bytes)
	{
		if (bytes == null)
		{
			throw new ArgumentNullException("bytes", "Environment.GetResourceString(\"ArgumentNull_Array\")");
		}
		return self.GetString(bytes, 0, bytes.Length);
	}

	public static string GetString(this UTF8Encoding self, byte[] bytes, int count)
	{
		return self.GetString(bytes, 0, count);
	}
}
