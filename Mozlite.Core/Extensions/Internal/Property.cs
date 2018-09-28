using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Mozlite.Extensions.Internal
{
    /// <summary>
    /// 属性类型。
    /// </summary>
    public class Property : IProperty
    {
        private IClrPropertyGetter _getter;
        private IClrPropertySetter _setter;
        /// <summary>
        /// 初始化类<see cref="Property"/>。
        /// </summary>
        /// <param name="info">属性信息实例。</param>
        /// <param name="entityType">实体类型。</param>
        public Property(PropertyInfo info, EntityType entityType)
        {
            Name = info.Name;
            PropertyInfo = info;
            ClrType = info.PropertyType;
            DeclaringType = entityType;
            if (info.IsDefined(typeof(IdentityAttribute)))
            {
                IsIdentity = true;
                entityType.Identity = this;
            }
            IsPrimaryKey = info.IsDefined(typeof(KeyAttribute));
            MaxLength = info.GetCustomAttribute<SizeAttribute>()?.MaximumLength;
            IsRowVersion = info.IsDefined(typeof(RowVersionAttribute));
            DisplayName = info.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ??
                          info.GetCustomAttribute<DisplayAttribute>()?.Name;
        }

        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 显示名称。
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// 类型。
        /// </summary>
        public Type ClrType { get; }

        /// <summary>
        /// 声明此属性的类型。
        /// </summary>
        public IEntityType DeclaringType { get; }

        /// <summary>
        /// 当前属性是否可以承载null值。
        /// </summary>
        public virtual bool IsNullable
        {
            get
            {
                if (IsIdentity || IsPrimaryKey)
                    return false;
                return ClrType.IsNullableType();
            }
        }

        /// <summary>
        /// 属性信息实例。
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// 版本列。
        /// </summary>
        public bool IsRowVersion { get; }

        /// <summary>
        /// 获取当前属性值。
        /// </summary>
        /// <param name="instance">当前对象实例。</param>
        /// <returns>获取当前属性值。</returns>
        public object Get(object instance)
        {
            return Getter.GetClrValue(instance);
        }

        /// <summary>
        /// 设置当前属性。
        /// </summary>
        /// <param name="instance">当前对象实例。</param>
        /// <param name="value">属性值。</param>
        public void Set(object instance, object value)
        {
            Setter.SetClrValue(instance, value);
        }

        /// <summary>
        /// 是否为自增长属性。
        /// </summary>
        public bool IsIdentity { get; }

        /// <summary>
        /// 最大长度。
        /// </summary>
        public int? MaxLength { get; }

        /// <summary>
        /// 是否为主键。
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 属性的获取访问器实例。
        /// </summary>
        protected virtual IClrPropertyGetter Getter
            => NonCapturingLazyInitializer.EnsureInitialized(ref _getter, this, p => new ClrPropertyGetterFactory().Create(p));

        /// <summary>
        /// 属性的设置访问器实例。
        /// </summary>
        protected virtual IClrPropertySetter Setter
            => NonCapturingLazyInitializer.EnsureInitialized(ref _setter, this, p => new ClrPropertySetterFactory().Create(p));

        /// <summary>
        /// 返回属性名。
        /// </summary>
        /// <returns>返回属性名。</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}