using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 用户登入提供者的一些信息存储。
    /// </summary>
    [Table("core_Users_Tokens")]
    public abstract class UserTokenBase
    {
        /// <summary>
        /// 用户ID。
        /// </summary>
        [Key]
        public int UserId { get; set; }

        /// <summary>
        /// 登入提供者。
        /// </summary>
        [Key]
        [Size(256)]
        public string LoginProvider { get; set; }

        /// <summary>
        /// 标识唯一键。
        /// </summary>
        [Key]
        [Size(256)]
        public string Name { get; set; }

        /// <summary>
        /// 当前标识的值。
        /// </summary>
        public string Value { get; set; }
    }
}