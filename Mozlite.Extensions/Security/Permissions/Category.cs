using Mozlite.Extensions.Categories;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Security.Permissions
{
    /// <summary>
    /// 权限分类。
    /// </summary>
    [Table("core_Permissions_Categories")]
    public class Category : CategoryBase
    {
        /// <summary>
        /// 显示文本（中文）。
        /// </summary>
        [Size(64)]
        public string Text { get; set; }

        /// <summary>
        /// 是否禁用。
        /// </summary>
        public bool Disabled { get; set; }
    }
}