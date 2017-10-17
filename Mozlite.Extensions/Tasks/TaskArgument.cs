using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Tasks
{
    /// <summary>
    /// 服务参数实例。
    /// </summary>
    [Table("core_Tasks_Arguments")]
    public class TaskArgument
    {
        /// <summary>
        /// 参数化Id。
        /// </summary>
        [Identity]
        public long Id { get; set; }

        /// <summary>
        /// 服务Id。
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// 类型。
        /// </summary>
        [Size(64)]
        public string ExtensionName { get; set; }

        /// <summary>
        /// 执行次数。
        /// </summary>
        public int TryTimes { get; set; }

        /// <summary>
        /// 参数。
        /// </summary>
        public string Argument { get; set; }

        /// <summary>
        /// 状态。
        /// </summary>
        public ArgumentStatus Status { get; set; }

        /// <summary>
        /// 最后一次执行时间。
        /// </summary>
        public DateTime LastExecuted { get; set; } = DateTime.Now;

        /// <summary>
        /// 错误消息。
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// 实际参数值。
        /// </summary>
        [NotMapped]
        public Argument Args
        {
            get => Argument;
            set => Argument = value.ToString();
        }
    }
}