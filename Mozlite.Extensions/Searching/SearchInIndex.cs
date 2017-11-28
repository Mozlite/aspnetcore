using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Searching
{
    /// <summary>
    /// 索引和搜索实体关联表。
    /// </summary>
    [Table("core_Searching_In_Indexes")]
    public class SearchInIndex
    {
        /// <summary>
        /// 检索实体Id。
        /// </summary>
        [Key]
        public long SearchId { get; set; }

        /// <summary>
        /// 索引ID。
        /// </summary>
        [Key]
        public long IndexId { get; set; }
    }
}