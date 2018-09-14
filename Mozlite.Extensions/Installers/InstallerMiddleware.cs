using Microsoft.AspNetCore.Http;
using Mozlite.Extensions.Tasks;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Installers
{
    /// <summary>
    /// 安装包判断。
    /// </summary>
    public class InstallerMiddleware
    {
        private readonly RequestDelegate _next;
        /// <summary>
        /// 初始化类<see cref="InstallerMiddleware"/>。
        /// </summary>
        /// <param name="next">下一个请求代理。</param>
        public InstallerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 执行方法。
        /// </summary>
        /// <param name="context">HTTP上下文。</param>
        /// <returns>返回当前任务。</returns>
        public async Task Invoke(HttpContext context)
        {
            var installer = await TaskHelper.GetInstallerUrlAsync();
            if (installer != null)
            {
                if (IsIgnoredFilter(context))
                    await _next(context);
                else
                    context.Response.Redirect(installer);
                return;
            }

            if (await InitAsync(context))
                await _next(context);
        }

        /// <summary>
        /// 执行方法。
        /// </summary>
        /// <param name="context">HTTP上下文。</param>
        /// <returns>返回当前任务。</returns>
        protected virtual Task<bool> InitAsync(HttpContext context)
        {
            return Task.FromResult(true);
        }

        private static readonly string[] _filters =
        {
            "/installer/",
            "/dist/",
            "/images/",
            "/js/",
            "/css/",
            "/favicon.ico/"
        };

        /// <summary>
        /// 判断是否可以忽略的路径。
        /// </summary>
        /// <param name="context">当前请求上下文。</param>
        /// <returns>返回判断结果。</returns>
        protected bool IsIgnoredFilter(HttpContext context)
        {
            var path = context.Request.Path.Value + '/';
            path = path.ToLower();
            foreach (var filter in _filters)
            {
                if (path.StartsWith(filter))
                    return true;
            }
            return false;
        }
    }
}