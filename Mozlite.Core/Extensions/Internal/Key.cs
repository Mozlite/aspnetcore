using System.Collections.Generic;

namespace Mozlite.Extensions.Internal
{
    /// <summary>
    /// 主键或者唯一键类。
    /// </summary>
    internal class Key : IKey
    {
        /// <summary>
        /// 初始化类<see cref="Key"/>。
        /// </summary>
        /// <param name="properties">键的属性列表。</param>
        public Key( IReadOnlyList<Property> properties)
        {
            Properties = properties;
        }

        /// <summary>
        /// 所包含的属性。
        /// </summary>
        public IReadOnlyList<IProperty> Properties { get; }

        /// <summary>
        /// 所属类型。
        /// </summary>
        public IEntityType DeclaringType => Properties[0].DeclaringType;
    }
}