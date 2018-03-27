using System;
using Mozlite.Extensions.Data;
using Mozlite.Extensions.Searching;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Documents
{
    /// <summary>
    /// 文档类型。
    /// </summary>
    [Table("core_Documents")]
    public class Document : IIdObject, ISearchable
    {
        /// <summary>
        /// 文档唯一Id。
        /// </summary>
        [Identity]
        public int Id { get; set; }

        /// <summary>
        /// 标题。
        /// </summary>
        [Size(256)]
        public string Title { get; set; }

        /// <summary>
        /// 文档内容MarkDown源码。
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 文档内容HTML源码。
        /// </summary>
        public string Html { get; set; }

        /// <summary>
        /// 添加时间。
        /// </summary>
        [NotUpdated]
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// 更新时间。
        /// </summary>
        public DateTimeOffset UpdatedDate { get; set; }

        /// <summary>
        /// 有帮助。
        /// </summary>
        public int Good { get; set; }

        /// <summary>
        /// 无帮助。
        /// </summary>
        public int Bad { get; set; }

        /// <summary>
        /// 标签。
        /// </summary>
        [Size(256)]
        public string Tags { get; set; }

        /// <summary>
        /// 搜索索引类型。
        /// </summary>
        public IndexedType SearchIndexed { get; set; }
    }
}
