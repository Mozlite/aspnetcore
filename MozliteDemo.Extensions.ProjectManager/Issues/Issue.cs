using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Mozlite.Extensions;

namespace MozliteDemo.Extensions.ProjectManager.Issues
{
    /// <summary>
    /// 任务。
    /// </summary>
    [Table("pm_Issues")]
    public class Issue : ExtendBase, IIdObject
    {
        /// <summary>
        /// 获取或设置唯一Id。
        /// </summary>
        [Identity]
        public int Id { get; set; }

        /// <summary>
        /// 父级Id。
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 类型。
        /// </summary>
        public IssueType Type { get; set; }

        /// <summary>
        /// 里程碑Id。
        /// </summary>
        public int MilestoneId { get; set; }

        /// <summary>
        /// 创建人。
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 当前操作员。
        /// </summary>
        public int Operator { get; set; }

        /// <summary>
        /// 验收员。
        /// </summary>
        public int Moderator { get; set; }

        /// <summary>
        /// 状态。
        /// </summary>
        public IssueStatus Status { get; set; }

        /// <summary>
        /// 标题。
        /// </summary>
        [Size(256)]
        public string Title { get; set; }

        /// <summary>
        /// 标签。
        /// </summary>
        [Size(256)]
        public string Tags { get; set; }

        /// <summary>
        /// MD源码。
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// HTML代码。
        /// </summary>
        public string Html { get; set; }

        /// <summary>
        /// 优先级。
        /// </summary>
        public IssueLevel Level { get; set; }

        /// <summary>
        /// 小时。
        /// </summary>
        public int Hours { get; set; }

        /// <summary>
        /// 添加时间。
        /// </summary>
        [NotUpdated]
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// 更新时间。
        /// </summary>
        public DateTimeOffset? UpdatedDate { get; set; }

        /// <summary>
        /// 开始时间。
        /// </summary>
        public DateTimeOffset? StartDate { get; set; }

        /// <summary>
        /// 完成时间。
        /// </summary>
        public DateTimeOffset? CompletedDate { get; set; }

        /// <summary>
        /// 项目Id。
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// 父级任务。
        /// </summary>
        [NotMapped]
        public Issue Parent { get; set; }

        /// <summary>
        /// 子级任务。
        /// </summary>
        [NotMapped]
        public IEnumerable<Issue> Children { get; set; }
    }
}