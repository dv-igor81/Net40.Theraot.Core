namespace System.Reflection;

public static class IntrospectionExtensions
{
    private static readonly TypeInfo Default = new TypeInfo(typeof(object));

    public static TypeInfo GetTypeInfo(this Type type)
    {
            return type == null ? Default : new TypeInfo(type);
        }
}