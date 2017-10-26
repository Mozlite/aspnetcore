using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mozlite.Extensions.Tasks;

namespace Mozlite.Extensions
{
    public class TaskService1 : TaskService
    {
        private readonly ILogger<TaskService1> _logger;
        public TaskService1(ILogger<TaskService1> logger)
        {
            _logger = logger;
        }

        public override string Name => "测试服务";
        public override string Description => "测试服务描述";

        public override TaskInterval Interval => 10;
        
        /// <summary>
        /// 执行方法。
        /// </summary>
        /// <param name="argument">参数。</param>
        public override Task ExecuteAsync(Argument argument)
        {
            _logger.LogInformation("运行了一次："+Name);
            return Task.FromResult(0);
        }
    }
}