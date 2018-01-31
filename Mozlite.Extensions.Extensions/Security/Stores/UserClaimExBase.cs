namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 用户声明类。
    /// </summary>
    public abstract class UserClaimExBase : UserClaimBase, ISitable
    {
        /// <summary>
        /// 获取当前站Id。
        /// </summary>
        public int SiteId { get; set; }
    }
}