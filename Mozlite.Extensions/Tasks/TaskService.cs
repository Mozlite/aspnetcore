using System.Threading.Tasks;

namespace Mozlite.Extensions.Tasks
{
    /// <summary>
    /// 后台服务基类。
    /// </summary>
    public abstract class TaskService : ITaskService
    {
        /// <summary>
        /// 优先级。
        /// </summary>
        public virtual int Priority => 0;

        /// <summary>
        /// 名称。
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 描述。
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// 扩展名称，即服务的分类。
        /// </summary>
        public virtual string ExtensionName => "core";

        /// <summary>
        /// 执行间隔时间。
        /// </summary>
        public abstract TaskInterval Interval { get; }

        /// <summary>
        /// 是否依赖参数，如果设置为true，如果数据库中没有参数则不执行。
        /// </summary>
        public virtual bool DependenceArgument => false;

        /// <summary>
        /// 执行方法。
        /// </summary>
        /// <param name="argument">参数。</param>
        public abstract Task ExecuteAsync(Argument argument);
    }
}