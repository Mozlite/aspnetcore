using System.Linq;
using System.Reflection;

namespace Mozlite.Extensions.Internal
{
    /// <summary>
    /// CLR访问器工厂基类。
    /// </summary>
    /// <typeparam name="TAccessor">当前类型接口。</typeparam>
    public abstract class ClrAccessorFactory<TAccessor>
        where TAccessor : class
    {
        private static readonly MethodInfo _genericCreate
            = typeof(ClrAccessorFactory<TAccessor>).GetTypeInfo().GetDeclaredMethods(nameof(CreateGeneric)).Single();

        /// <summary>
        /// 获取属性的CLR访问器实例。
        /// </summary>
        /// <param name="property">属性实例。</param>
        /// <returns>返回CLR访问器实例。</returns>
        public virtual TAccessor Create( Property property)
            => property as TAccessor ?? Create(property.PropertyInfo);

        /// <summary>
        /// 获取属性的CLR访问器实例。
        /// </summary>
        /// <param name="propertyInfo">属性实例。</param>
        /// <returns>返回CLR访问器实例。</returns>
        public virtual TAccessor Create( PropertyInfo propertyInfo)
        {
            var boundMethod = _genericCreate.MakeGenericMethod(
                    propertyInfo.DeclaringType,
                    propertyInfo.PropertyType,
                    propertyInfo.PropertyType.UnwrapNullableType());

            try
            {
                return (TAccessor)boundMethod.Invoke(this, new object[] { propertyInfo });
            }
            catch (TargetInvocationException e) when (e.InnerException != null)
            {
                throw e.InnerException;
            }
        }

        /// <summary>
        /// 创建当前访问器实例。
        /// </summary>
        /// <typeparam name="TEntity">当前实体类型。</typeparam>
        /// <typeparam name="TValue">属性的CLR类型。</typeparam>
        /// <typeparam name="TNonNullableEnumValue">此属性的基础类型。</typeparam>
        /// <param name="propertyInfo">属性信息实例。</param>
        /// <returns>返回当前属性的访问器实例。</returns>
        protected abstract TAccessor CreateGeneric<TEntity, TValue, TNonNullableEnumValue>(
             PropertyInfo propertyInfo)
            where TEntity : class;
    }
}