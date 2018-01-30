using System.ComponentModel.DataAnnotations;

namespace Mozlite.Extensions.Categories
{
    /// <summary>
    /// 分类Id。
    /// </summary>
    public abstract class CategoryExBase : CategoryBase, ISitable
    {
        /// <summary>
        /// 获取当前站Id。
        /// </summary>
        [Key]
        public int SiteId { get; set; }
    }
}