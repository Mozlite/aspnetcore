using System;
using System.Reflection;

namespace Mozlite.Extensions
{
    /// <summary>
    /// 属性接口。
    /// </summary>
    public interface IProperty
    {
        /// <summary>
        /// 名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 显示名称。
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// 类型。
        /// </summary>
        Type ClrType { get; }

        /// <summary>
        /// 声明此属性的类型。
        /// </summary>
        IEntityType DeclaringType { get; }

        /// <summary>
        /// 属性信息实例。
        /// </summary>
        PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// 当前属性是否可以承载null值。
        /// </summary>
        bool IsNullable { get; }

        /// <summary>
        /// 是否为自增长属性。
        /// </summary>
        bool IsIdentity { get; }

        /// <summary>
        /// 最大长度。
        /// </summary>
        int? MaxLength { get; }
        
        /// <summary>
        /// 版本列。
        /// </summary>
        bool IsRowVersion { get; }

        /// <summary>
        /// 获取当前属性值。
        /// </summary>
        /// <param name="instance">当前对象实例。</param>
        /// <returns>获取当前属性值。</returns>
        object Get(object instance);

        /// <summary>
        /// 设置当前属性。
        /// </summary>
        /// <param name="instance">当前对象实例。</param>
        /// <param name="value">属性值。</param>
        void Set(object instance, object value);
    }
}