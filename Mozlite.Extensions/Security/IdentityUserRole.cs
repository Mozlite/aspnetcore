using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 用户和用户组。
    /// </summary>
    [Table("core_Users_Roles")]
    public class IdentityUserRole
    {
        /// <summary>
        /// 用户组ID。
        /// </summary>
        [Key]
        public int RoleId { get; set; }

        /// <summary>
        /// 用户ID。
        /// </summary>
        [Key]
        public int UserId { get; set; }
    }
}