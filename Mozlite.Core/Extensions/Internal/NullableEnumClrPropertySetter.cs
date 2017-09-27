using System;

namespace Mozlite.Extensions.Internal
{
    /// <summary>
    /// 枚举属性设置。
    /// </summary>
    /// <typeparam name="TEntity">实体类型。</typeparam>
    /// <typeparam name="TValue">属性值。</typeparam>
    /// <typeparam name="TNonNullableEnumValue">基础类型。</typeparam>
    public class NullableEnumClrPropertySetter<TEntity, TValue, TNonNullableEnumValue> : IClrPropertySetter
        where TEntity : class
    {
        private readonly Action<TEntity, TValue> _setter;

        /// <summary>
        /// 初始化类<see cref="NullableEnumClrPropertySetter{TEntity, TValue, TNonNullableEnumValue}"/>。
        /// </summary>
        /// <param name="setter">设置代理方法。</param>
        public NullableEnumClrPropertySetter( Action<TEntity, TValue> setter)
        {
            _setter = setter;
        }

        /// <summary>
        /// 设置当前属性的值。
        /// </summary>
        /// <param name="instance">当前对象实例。</param>
        /// <param name="value">属性值。</param>
        public virtual void SetClrValue(object instance, object value)
        {
            if (value != null)
            {
                value = (TNonNullableEnumValue)value;
            }

            _setter((TEntity)instance, (TValue)value);
        }
    }
}