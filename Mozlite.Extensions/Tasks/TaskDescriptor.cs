using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

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
        /// 是否依赖参数配置。
        /// </summary>
        public bool DependenceArgument { get; set; }

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
        /// 前一次执行时间。
        /// </summary>
        public DateTime? LastExecuted { get; set; }

        /// <summary>
        /// 下一次执行时间。
        /// </summary>
        public DateTime NextExecuting { get; set; }

        /// <summary>
        /// 执行间隔。
        /// </summary>
        [NotMapped]
        public TaskInterval TaskInterval
        {
            get => Interval;
            set => Interval = value?.ToString();
        }

        internal Func<Argument, Task> Service { get; set; }

        internal bool IsRunning { get; set; }
    }
}