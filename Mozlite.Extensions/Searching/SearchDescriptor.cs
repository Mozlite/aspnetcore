using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Searching
{
    /// <summary>
    /// 搜索分词表。
    /// </summary>
    [Table("core_Searchings")]
    public class SearchDescriptor
    {
        /// <summary>
        /// 唯一Id。
        /// </summary>
        [Identity]
        public long Id { get; set; }

        /// <summary>
        /// 检索目标ID。
        /// </summary>
        public int TargetId { get; set; }
        
        /// <summary>
        /// 提供者名称。
        /// </summary>
        [Size(64)]
        public string ProviderName { get; set; }

        /// <summary>
        /// 生成的HTML实体内容。
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 生成搜索实体的时间。
        /// </summary>
        public DateTime IndexedDate { get; set; }
        
        /// <summary>
        /// 返回生成的HTML实体内容。
        /// </summary>
        /// <returns>返回实体内容。</returns>
        public override string ToString()
        {
            return Summary;
        }
    }
}