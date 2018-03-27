using System;
using Mozlite.Extensions.Groups;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Documents
{
    /// <summary>
    /// 分类。
    /// </summary>
    [Table("core_Documents_Categories")]
    public class Category: GroupBase<Category>
    {
        /// <summary>
        /// 显示名称。
        /// </summary>
        [Size(64)]
        public string Title { get; set; }

        /// <summary>
        /// 排序。
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 描述。
        /// </summary>
        [Size(512)]
        public string Description { get; set; }
    }
}
