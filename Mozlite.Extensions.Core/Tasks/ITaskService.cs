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
        /// 禁用。
        /// </summary>
        bool Disabled { get; }

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
        /// 执行方法。
        /// </summary>
        /// <param name="argument">参数。</param>
        Task ExecuteAsync(Argument argument);
    }
}