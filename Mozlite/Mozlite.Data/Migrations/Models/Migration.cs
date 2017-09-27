using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Data.Migrations.Models
{
    /// <summary>
    /// 数据库实例。
    /// </summary>
    [Table("core_Migrations")]
    public class Migration
    {
        /// <summary>
        /// 迁移类型。
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// 版本。
        /// </summary>
        public int Version { get; set; }
    }
}