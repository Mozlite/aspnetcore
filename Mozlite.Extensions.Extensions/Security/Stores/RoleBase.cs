namespace Mozlite.Extensions.Extensions.Security.Stores
{
    /// <summary>
    /// 用户组基类。
    /// </summary>
    public abstract class RoleBase : Mozlite.Extensions.Security.Stores.RoleBase, ISitable
    {
        /// <summary>
        /// 获取当前站Id。
        /// </summary>
        public int SiteId { get; set; }
    }
}