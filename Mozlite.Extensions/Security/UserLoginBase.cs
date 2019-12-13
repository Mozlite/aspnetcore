using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 用户登录实例。
    /// </summary>
    [Table("core_Users_Logins")]
    public abstract class UserLoginBase : IdentityUserLogin<int>
    {
        /// <summary>
        /// 登录提供者(如：facebook, google)。
        /// </summary>
        [Key]
        [Size(64)]
        public override string LoginProvider { get; set; }

        /// <summary>
        /// 获取登录提供者提供的唯一Id。
        /// </summary>
        [Key]
        [Size(256)]
        public override string ProviderKey { get; set; }

        /// <summary>
        /// 登录提供者友好名称。
        /// </summary>
        [Size(64)]
        public override string ProviderDisplayName { get; set; }

        /// <summary>
        /// 用户登录的用户ID。
        /// </summary>
        public override int UserId { get; set; }
    }
}