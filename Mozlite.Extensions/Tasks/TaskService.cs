using System;
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
        /// 禁用。
        /// </summary>
        public virtual bool Disabled => false;

        /// <summary>
        /// 名称。
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 描述。
        /// </summary>
        public abstract string Description { get; }

        private string _extensionName;
        /// <summary>
        /// 扩展名称，即服务的分类。
        /// </summary>
        public virtual string ExtensionName
        {
            get
            {
                if (_extensionName == null)
                {
                    const string extensionName = ".Extensions.";
                    var @namespace = GetType().Namespace;
                    var index = @namespace.IndexOf(extensionName, StringComparison.Ordinal);
                    if (index == -1)
                        _extensionName = "core";
                    @namespace = @namespace.Substring(index + extensionName.Length);
                    index = @namespace.IndexOf(".", StringComparison.Ordinal);
                    if (index != -1)
                        @namespace = @namespace.Substring(0, index);
                    _extensionName = @namespace.ToLower();
                }
                return _extensionName;
            }
        }

        /// <summary>
        /// 执行间隔时间。
        /// </summary>
        public abstract TaskInterval Interval { get; }

        /// <summary>
        /// 执行方法。
        /// </summary>
        /// <param name="argument">参数。</param>
        public abstract Task ExecuteAsync(Argument argument);
    }
}