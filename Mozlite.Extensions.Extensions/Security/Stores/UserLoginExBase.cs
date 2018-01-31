using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 用户登入实例。
    /// </summary>
    public abstract class UserLoginExBase : UserLoginBase, ISitable
    {
        /// <summary>
        /// 获取当前站Id。
        /// </summary>
        [Key]
        public int SiteId { get; set; }
    }
}