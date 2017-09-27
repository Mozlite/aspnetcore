using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Mozlite.Extensions.Internal
{
    [DebuggerStepThrough]
    internal static class PropertyInfoExtensions
    {
        public static PropertyInfo FindGetterProperty( this PropertyInfo propertyInfo)
            => propertyInfo.DeclaringType
                .GetPropertiesInHierarchy(propertyInfo.Name)
                .FirstOrDefault(p => p.GetMethod != null);

        public static PropertyInfo FindSetterProperty( this PropertyInfo propertyInfo)
            => propertyInfo.DeclaringType
                .GetPropertiesInHierarchy(propertyInfo.Name)
                .FirstOrDefault(p => p.SetMethod != null);
    }
}