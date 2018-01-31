using System.ComponentModel.DataAnnotations;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 用户登入提供者的一些信息存储。
    /// </summary>
    public abstract class UserTokenExBase : UserTokenBase, ISitable
    {
        /// <summary>
        /// 获取当前站Id。
        /// </summary>
        [Key]
        public int SiteId { get; set; }
    }
}