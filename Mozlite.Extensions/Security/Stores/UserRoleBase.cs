using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 用户和角色。
    /// </summary>
    [Table("core_Users_Roles")]
    public abstract class UserRoleBase : IUserRole
    {
        /// <summary>
        /// 角色ID。
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