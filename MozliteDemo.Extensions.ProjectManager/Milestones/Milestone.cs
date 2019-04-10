using System;
using System.ComponentModel.DataAnnotations.Schema;
using Mozlite.Extensions;
using Mozlite.Extensions.Categories;

namespace MozliteDemo.Extensions.ProjectManager.Milestones
{
    /// <summary>
    /// 里程碑。
    /// </summary>
    [Table("pm_Milestones")]
    public class Milestone : CategoryBase
    {
        /// <summary>
        /// 项目Id。
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// 创建人。
        /// </summary>
        [NotUpdated]
        public int UserId { get; set; }

        /// <summary>
        /// 负责人。
        /// </summary>
        public int Operator { get; set; }

        /// <summary>
        /// 添加时间。
        /// </summary>
        [NotUpdated]
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// 开始时间。
        /// </summary>
        public DateTimeOffset? StartDate { get; set; }

        /// <summary>
        /// 完成时间。
        /// </summary>
        public DateTimeOffset? CompletedDate { get; set; }

        /// <summary>
        /// 描述。
        /// </summary>
        [Size(256)]
        public string Summary { get; set; }

        /// <summary>
        /// 任务数。
        /// </summary>
        [NotUpdated]
        public int Issues { get; set; }

        /// <summary>
        /// 完成数。
        /// </summary>
        [NotUpdated]
        public int Completed { get; set; }

        /// <summary>
        /// 完成度。
        /// </summary>
        public int Progress => Issues == 0 ? 0 : (int)Math.Ceiling(Completed * 100.0 / Issues);
    }
}