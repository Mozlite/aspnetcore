using System.ComponentModel.DataAnnotations;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 用户和用户组。
    /// </summary>
    public abstract class UserRoleExBase : UserRoleBase, IUserRoleEx
    {
        /// <summary>
        /// 获取当前站Id。
        /// </summary>
        [Key]
        public int SiteId { get; set; }
    }
}