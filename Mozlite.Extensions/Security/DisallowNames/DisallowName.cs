using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Security.DisallowNames
{
    /// <summary>
    /// 非法用户名。
    /// </summary>
    [Table("core_Users_Disallows")]
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
        [Size(20)]
        public string Name { get; set; }
    }
}