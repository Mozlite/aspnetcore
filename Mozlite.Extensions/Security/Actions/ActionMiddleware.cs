using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Mozlite.Data;
using Mozlite.Extensions.Security.Stores;
using System;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Security.Actions
{
    /// <summary>
    /// 操作中间件。
    /// </summary>
    /// <typeparam name="TUser">用户操作中间件。</typeparam>
    public class ActionMiddleware<TUser>
        where TUser : UserBase
    {
        private readonly RequestDelegate _next;
        private readonly IActionFactory _factory;

        /// <summary>
        /// 初始化类<see cref="ActionMiddleware"/>。
        /// </summary>
        /// <param name="next">下一个请求代理。</param>
        /// <param name="factory">操作工厂接口。</param>
        public ActionMiddleware(RequestDelegate next, IActionFactory factory)
        {
            _next = next;
            _factory = factory;
        }

        /// <summary>
        /// 执行方法。
        /// </summary>
        /// <param name="context">HTTP上下文。</param>
        /// <returns>返回当前任务。</returns>
        public async Task Invoke(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var user = await GetUserAsync(context);
                if (user != null &&
                    user.Action > 0 && _factory.TryGetProvider(user.Action, out var provider) &&
                    await provider.InvokeAsync(context))
                    return;
            }
            await _next(context);
        }

        private readonly Type _currentUserCacheKey = typeof(TUser);
        private async Task<TUser> GetUserAsync(HttpContext context)
        {
            if (context.Items.TryGetValue(_currentUserCacheKey, out object user) && user is TUser current)
                return current;
            var userId = context.User.GetUserId();
            if (userId > 0)
                current = await context.RequestServices.GetRequiredService<IDbContext<TUser>>().FindAsync(userId);
            else
                current = null;
            context.Items[_currentUserCacheKey] = current;
            return current;
        }
    }
}