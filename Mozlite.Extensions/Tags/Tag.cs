using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Tags
{
    /// <summary>
    /// 标签。
    /// </summary>
    [Table("core_Tags")]
    public class Tag
    {
        /// <summary>
        /// 唯一Id。
        /// </summary>
        [Identity]
        public int Id { get; set; }

        /// <summary>
        /// 名称。
        /// </summary>
        [Size(32)]
        public string Name { get; set; }

        /// <summary>
        /// 显示名称。
        /// </summary>
        [Size(32)]
        public string DisplayName { get; set; }

        /// <summary>
        /// 描述。
        /// </summary>
        [Size(256)]
        public string Description { get; set; }
    }
}