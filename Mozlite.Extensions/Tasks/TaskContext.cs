using System;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Tasks
{
    /// <summary>
    /// 后台服务上下文。
    /// </summary>
    public class TaskContext
    {
        /// <summary>
        /// 服务Id。
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否在运行。
        /// </summary>
        internal bool IsRunning { get; set; }

        /// <summary>
        /// 前一次执行时间。
        /// </summary>
        public DateTime? LastExecuted { get; set; }

        /// <summary>
        /// 下一次执行时间。
        /// </summary>
        public DateTime NextExecuting { get; set; }

        /// <summary>
        /// 执行间隔时间。
        /// </summary>
        public TaskInterval Interval { get; set; }

        private Argument _argument;
        /// <summary>
        /// 参数。
        /// </summary>
        public Argument Argument
        {
            get => _argument ?? new Argument();
            set
            {
                _argument = value;
                if (value != null)
                {
                    _argument.TaskContext = this;
                    if (!string.IsNullOrEmpty(_argument.Interval))
                        Interval = _argument.Interval;
                }
            }
        }

        /// <summary>
        /// 执行方法。
        /// </summary>
        internal Func<Argument, Task> ExecuteAsync { get; set; }
    }
}