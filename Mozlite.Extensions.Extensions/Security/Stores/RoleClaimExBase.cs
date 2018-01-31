namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 用户组声明类。
    /// </summary>
    public abstract class RoleClaimExBase : RoleClaimBase, ISitable
    {
        /// <summary>
        /// 获取当前站Id。
        /// </summary>
        public int SiteId { get; set; }
    }
}