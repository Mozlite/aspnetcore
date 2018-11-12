using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Extensions.Installers
{
    /// <summary>
    /// 安装包判断。
    /// </summary>
    public class InstallerMiddleware : Mozlite.Extensions.Installers.InstallerMiddleware
    {
        /// <summary>
        /// 执行方法。
        /// </summary>
        /// <param name="context">HTTP上下文。</param>
        /// <returns>返回当前任务。</returns>
        protected override Task<bool> InitAsync(HttpContext context)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// 初始化类<see cref="InstallerMiddleware"/>。
        /// </summary>
        /// <param name="next">下一个请求代理。</param>
        public InstallerMiddleware(RequestDelegate next) : base(next)
        {
        }
    }
}