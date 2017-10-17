using System.Threading.Tasks;

namespace Mozlite.Extensions.Tasks
{
    /// <summary>
    /// 后台执行的接口。
    /// </summary>
    public interface ITaskService : ISingletonServices
    {
        /// <summary>
        /// 优先级。
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// 名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 描述。
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 扩展名称，即服务的分类。
        /// </summary>
        string ExtensionName { get; }

        /// <summary>
        /// 执行间隔时间。
        /// </summary>
        TaskInterval Interval { get; }

        /// <summary>
        /// 是否依赖参数，如果设置为true，如果数据库中没有参数则不执行。
        /// </summary>
        bool DependenceArgument { get; }

        /// <summary>
        /// 执行方法。
        /// </summary>
        /// <param name="argument">参数。</param>
        Task ExecuteAsync(Argument argument);
    }
}