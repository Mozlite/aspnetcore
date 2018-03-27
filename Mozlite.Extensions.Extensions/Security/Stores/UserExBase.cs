using Mozlite.Extensions.Security.Stores;

namespace Mozlite.Extensions.Extensions.Security.Stores
{
    /// <summary>
    /// 用户基类。
    /// </summary>
    public abstract class UserExBase : UserBase, ISitable
    {
        /// <summary>
        /// 获取当前站Id。
        /// </summary>
        public int SiteId { get; set; }
    }
}