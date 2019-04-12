using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 包含用户字段基类。
    /// </summary>
    public abstract class UserFieldBase
    {
        /// <summary>
        /// 用户名称。
        /// </summary>
        [NotMapped]
        public string UserName { get; set; }

        /// <summary>
        /// 头像。
        /// </summary>
        [NotMapped]
        public string Avatar { get; set; }

        /// <summary>
        /// 登录名称。
        /// </summary>
        [NotMapped]
        public string NormalizedUserName { get; set; }

        /// <summary>
        /// 登录名称。
        /// </summary>
        public string LoginName => NormalizedUserName?.ToLower();

        /// <summary>
        /// 角色名称。
        /// </summary>
        [NotMapped]
        public int RoleId { get; set; }

        /// <summary>
        /// 角色名称。
        /// </summary>
        [NotMapped]
        public string RoleName { get; set; }
    }
}