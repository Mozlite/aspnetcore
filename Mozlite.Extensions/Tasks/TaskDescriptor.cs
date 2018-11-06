using Microsoft.AspNetCore.Html;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Tasks
{
    /// <summary>
    /// 后台任务。
    /// </summary>
    [Table("core_Tasks")]
    public class TaskDescriptor
    {
        /// <summary>
        /// 服务Id。
        /// </summary>
        [Identity]
        public int Id { get; set; }

        /// <summary>
        /// 名称。
        /// </summary>
        [Size(64)]
        public string Name { get; set; }

        /// <summary>
        /// 描述。
        /// </summary>
        [Size(256)]
        public string Description { get; set; }

        /// <summary>
        /// 所属扩展类型，一般为类型Mozlite.Extensions.名称。
        /// </summary>
        [Size(64)]
        public string ExtensionName { get; set; }

        /// <summary>
        /// 任务执行类型。
        /// </summary>
        [Size(256)]
        public string Type { get; set; }

        /// <summary>
        /// 执行间隔。
        /// </summary>
        [Size(64)]
        public string Interval { get; set; }

        /// <summary>
        /// 显示间隔。
        /// </summary>
        public IHtmlContent ToHtmlInterval()
        {
            TaskInterval interval;
            if (string.IsNullOrEmpty(TaskArgument.Interval))
                interval = Interval;
            else
                interval = TaskArgument.Interval;
            return interval.ToHtmlString();
        }

        /// <summary>
        /// 前一次执行时间。
        /// </summary>
        public DateTime? LastExecuted { get; set; }

        /// <summary>
        /// 下一次执行时间。
        /// </summary>
        public DateTime NextExecuting { get; set; } = DateTime.Now;

        /// <summary>
        /// 是否启用。
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 参数。
        /// </summary>
        public string Argument { get; set; }

        private Argument _argument;
        /// <summary>
        /// 参数实例。
        /// </summary>
        public Argument TaskArgument => _argument ?? (_argument = new Argument(Argument));

        /// <summary>
        /// 是否需要被删除。
        /// </summary>
        internal bool ShouldBeDeleting { get; set; } = true;
    }
}