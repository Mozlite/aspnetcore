using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Security.Permissions
{
    /// <summary>
    /// 角色权限。
    /// </summary>
    [Table("core_Permissions_In_Roles")]
    public class PermissionInRole
    {
        /// <summary>
        /// 角色Id。
        /// </summary>
        [Key]
        public int RoleId { get; set; }

        /// <summary>
        /// 权限Id。
        /// </summary>
        [Key]
        public int PermissionId { get; set; }

        /// <summary>
        /// 当前权限设定。
        /// </summary>
        public PermissionValue Value { get; set; }
    }
}