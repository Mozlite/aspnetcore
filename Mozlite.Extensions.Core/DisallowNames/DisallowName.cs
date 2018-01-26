using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.DisallowNames
{
    /// <summary>
    /// 非法用户名。
    /// </summary>
    [Table("core_DisallowNames")]
    public class DisallowName
    {
        /// <summary>
        /// 唯一Id。
        /// </summary>
        [Identity]
        public int Id { get; set; }

        /// <summary>
        /// 非法用户名。
        /// </summary>
        [Key]
        [Size(20)]
        public string Name { get; set; }
    }
}