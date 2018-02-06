using Mozlite.Extensions.Data;

namespace Mozlite.Extensions.Categories
{
    /// <summary>
    /// 分类Id。
    /// </summary>
    public abstract class CategoryExBase : CategoryBase, IIdSiteObject
    {
        /// <summary>
        /// 获取当前站Id。
        /// </summary>
        public int SiteId { get; set; }
    }
}