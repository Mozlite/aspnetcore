using System;

namespace Mozlite.Extensions.Internal
{
    /// <summary>
    /// 属性获取访问器实现类。
    /// </summary>
    /// <typeparam name="TEntity">当前实体类型。</typeparam>
    /// <typeparam name="TValue">属性类型。</typeparam>
    public class ClrPropertyGetter<TEntity, TValue> : IClrPropertyGetter
        where TEntity : class
    {
        private readonly Func<TEntity, TValue> _getter;

        /// <summary>
        /// 初始化类<see cref="ClrPropertyGetter{TEntity, TValue}"/>。
        /// </summary>
        /// <param name="getter">当前访问器执行的方法代理。</param>
        public ClrPropertyGetter( Func<TEntity, TValue> getter)
        {
            _getter = getter;
        }

        /// <summary>
        /// 获取当前对象中属性的值。
        /// </summary>
        /// <param name="instance">当前对象实例。</param>
        /// <returns>返回属性值。</returns>
        public virtual object GetClrValue(object instance) => _getter((TEntity)instance);
    }
}