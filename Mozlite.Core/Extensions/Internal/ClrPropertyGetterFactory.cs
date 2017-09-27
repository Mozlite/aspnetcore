using System;
using System.Linq.Expressions;
using System.Reflection;
using Mozlite.Properties;

namespace Mozlite.Extensions.Internal
{
    /// <summary>
    /// 属性获取工厂类。
    /// </summary>
    public class ClrPropertyGetterFactory : ClrAccessorFactory<IClrPropertyGetter>
    {
        /// <summary>
        /// 创建当前访问器实例。
        /// </summary>
        /// <typeparam name="TEntity">当前实体类型。</typeparam>
        /// <typeparam name="TValue">属性的CLR类型。</typeparam>
        /// <typeparam name="TNonNullableEnumValue">此属性的基础类型。</typeparam>
        /// <param name="propertyInfo">属性信息实例。</param>
        /// <returns>返回当前属性的访问器实例。</returns>
        protected override IClrPropertyGetter CreateGeneric<TEntity, TValue, TNonNullableEnumValue>(PropertyInfo propertyInfo)
        {
            var memberInfo = propertyInfo.FindGetterProperty();

            if (memberInfo == null)
            {
                throw new InvalidOperationException(string.Format(
                    Resources.NoGetter, propertyInfo.Name, propertyInfo.DeclaringType.DisplayName(false)));
            }

            var entityParameter = Expression.Parameter(typeof(TEntity), "entity");

            return new ClrPropertyGetter<TEntity, TValue>(Expression.Lambda<Func<TEntity, TValue>>(
                Expression.MakeMemberAccess(entityParameter, memberInfo),
                entityParameter).Compile());
        }
    }
}