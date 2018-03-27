using Mozlite.Extensions.Extensions.Data;

namespace Mozlite.Extensions.Extensions.Categories
{
    /// <summary>
    /// 分类Id。
    /// </summary>
    public abstract class CategoryBase : Mozlite.Extensions.Categories.CategoryBase, ISitableObject
    {
        /// <summary>
        /// 获取当前站Id。
        /// </summary>
        public int SiteId { get; set; }
    }
}