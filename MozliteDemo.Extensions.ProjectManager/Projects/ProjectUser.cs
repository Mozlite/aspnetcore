using Mozlite.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MozliteDemo.Extensions.Security;

namespace MozliteDemo.Extensions.ProjectManager.Projects
{
    /// <summary>
    /// 用户。
    /// </summary>
    [Table("pm_Users")]
    public class ProjectUser : UserFieldBase, IIdObject
    {
        /// <summary>
        /// ID。
        /// </summary>
        [Key]
        public int Id { get; set; }
    }
}
