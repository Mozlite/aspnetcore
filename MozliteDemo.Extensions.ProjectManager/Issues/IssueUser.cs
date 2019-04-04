using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MozliteDemo.Extensions.ProjectManager.Issues
{
    /// <summary>
    /// 参与的用户。
    /// </summary>
    [Table("pm_Issues_Users")]
    public class IssueUser
    {
        /// <summary>
        /// 用户Id。
        /// </summary>
        [Key]
        public int UserId { get; set; }

        /// <summary>
        /// 任务Id。
        /// </summary>
        [Key]
        public int IssueId { get; set; }
    }
}