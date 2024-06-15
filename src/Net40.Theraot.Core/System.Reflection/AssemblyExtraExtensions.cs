using System.Collections.Generic;

namespace System.Reflection;

public static class AssemblyExtraExtensions
{
    public static bool ReflectionOnly(this Assembly self)
    {
            return self.ReflectionOnly;
        }
        
    public static IEnumerable<TypeInfo> DefinedTypes(this Assembly self)
    {
            Type[] types = self.GetTypes();
            TypeInfo[] array = new TypeInfo[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                TypeInfo typeInfo = types[i].GetTypeInfo();
                if (typeInfo == null)
                {
                    throw new NotSupportedException("SR.Format(SR.NotSupported_NoTypeInfo, types[i].FullName)");
                }
                array[i] = typeInfo;
            }
            return array;
        }
}