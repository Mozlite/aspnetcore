using System;

namespace Mozlite.Extensions.Internal
{
    /// <summary>
    /// CLR属性的Setter方法接口。
    /// </summary>
    public interface IClrPropertySetter
    {
        /// <summary>
        /// 设置当前属性的值。
        /// </summary>
        /// <param name="instance">当前对象实例。</param>
        /// <param name="value">属性值。</param>
        void SetClrValue( object instance,  object value);
    }

    /// <summary>
    /// 属性设置访问器实现类。
    /// </summary>
    /// <typeparam name="TEntity">实体类型。</typeparam>
    /// <typeparam name="TValue">属性值类型。</typeparam>
    public class ClrPropertySetter<TEntity, TValue> : IClrPropertySetter
        where TEntity : class
    {
        private readonly Action<TEntity, TValue> _setter;

        /// <summary>
        /// 初始化类<see cref="ClrPropertySetter{TEntity, TValue}"/>。
        /// </summary>
        /// <param name="setter">属性设置代理方法。</param>
        public ClrPropertySetter( Action<TEntity, TValue> setter)
        {
            _setter = setter;
        }

        /// <summary>
        /// 设置当前属性的值。
        /// </summary>
        /// <param name="instance">当前对象实例。</param>
        /// <param name="value">属性值。</param>
        public virtual void SetClrValue(object instance, object value)
            => _setter((TEntity)instance, (TValue)value);
    }
}