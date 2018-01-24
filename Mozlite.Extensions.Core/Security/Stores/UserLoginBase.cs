using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 用户登入实例。
    /// </summary>
    [Table("core_Users_Logins")]
    public abstract class UserLoginBase
    {
        /// <summary>
        /// 登入提供者(如：facebook, google)。
        /// </summary>
        [Key]
        [Size(64)]
        public string LoginProvider { get; set; }

        /// <summary>
        /// 获取登入提供者提供的唯一Id。
        /// </summary>
        [Key]
        [Size(256)]
        public string ProviderKey { get; set; }

        /// <summary>
        /// 登入提供者友好名称。
        /// </summary>
        [Size(64)]
        public string ProviderDisplayName { get; set; }

        /// <summary>
        /// 用户登入的用户ID。
        /// </summary>
        public int UserId { get; set; }
    }
}