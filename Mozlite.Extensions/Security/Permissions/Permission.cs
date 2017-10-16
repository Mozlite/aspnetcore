using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Security.Permissions
{
    /// <summary>
    /// 权限实体。
    /// </summary>
    [Table("core_Permissions")]
    public class Permission
    {
        /// <summary>
        /// 唯一Id。
        /// </summary>
        [Identity]
        public int Id { get; set; }

        /// <summary>
        /// 名称。
        /// </summary>
        [Size(64)]
        public string Name { get; set; }

        /// <summary>
        /// 描述。
        /// </summary>
        [Size(256)]
        public string Description { get; set; }
    }
}