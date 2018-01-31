namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 角色基类。
    /// </summary>
    public abstract class RoleExBase : RoleBase, ISitable
    {
        /// <summary>
        /// 获取当前站Id。
        /// </summary>
        public int SiteId { get; set; }
    }
}