using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 用户和角色。
    /// </summary>
    [Table("core_Users_Roles")]
    public abstract class UserRoleBase : IdentityUserRole<int>,IUserRole
    {
        /// <summary>
        /// 角色ID。
        /// </summary>
        [Key]
        public override int RoleId { get; set; }

        /// <summary>
        /// 用户ID。
        /// </summary>
        [Key]
        public override int UserId { get; set; }
    }
}