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
        /// 前一次执行时间。
        /// </summary>
        public DateTime? LastExecuted { get; set; }

        /// <summary>
        /// 下一次执行时间。
        /// </summary>
        public DateTime NextExecuting { get; set; }

        /// <summary>
        /// 是否启用。
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 参数。
        /// </summary>
        [Size(256)]
        public string Argument { get; set; }

        /// <summary>
        /// 错误消息。
        /// </summary>
        public string Error { get; set; }
    }
}