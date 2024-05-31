namespace System.Reflection;

public static class ParameterInfoTheraotExtensions
{
	public static bool HasDefaultValue(this ParameterInfo @this)
	{
		object defaultValue = @this.DefaultValue;
		return defaultValue != DBNull.Value && defaultValue != Type.Missing;
	}
}
