namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 分类。
    /// </summary>
    public abstract class CategoryExBase : CategoryBase, ISitable
    {
        /// <summary>
        /// 获取当前站Id。
        /// </summary>
        public int SiteId { get; set; }
    }
}