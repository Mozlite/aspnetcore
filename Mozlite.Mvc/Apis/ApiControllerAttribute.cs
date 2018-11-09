using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// API特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiControllerAttribute : Microsoft.AspNetCore.Mvc.ApiControllerAttribute, IAuthorizationFilter
    {
        private const string AppId = "appid";
        private bool TryGetValue(HttpRequest request, string key, out StringValues value)
        {
            if (request.Headers.TryGetValue($"x-{key}", out value) || request.Query.TryGetValue(key, out value))
                return true;
            value = StringValues.Empty;
            return false;
        }

        private JsonResult Error(string name) => new JsonResult(new ApiResult(ErrorCode.InvalidParameter).Format(name));

        private JsonResult Error(ErrorCode code) => new JsonResult(new ApiResult(code));

        /// <summary>
        /// 判断当前是请求否已经验证。
        /// </summary>
        /// <param name="context">验证过滤器<see cref="T:Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext" />上下文。</param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!TryGetValue(context.HttpContext.Request, AppId, out var value) || !Guid.TryParse(value, out var appid))
            {//获取appid失败
                context.Result = Error(AppId);
                return;
            }
            var application = context.HttpContext.RequestServices.GetRequiredService<IApiManager>().Find(appid);
            if (application == null)
            {//获取应用失败
                context.Result = Error(AppId);
                return;
            }
            context.HttpContext.Items[typeof(CacheApplication)] = application;
            if (Anonymousable)//无需验证
                return;
            if (string.IsNullOrEmpty(application.Token) || application.ExpiredDate <= DateTimeOffset.Now)
                context.Result = Error(ErrorCode.InvalidToken);
            else if (!TryGetValue(context.HttpContext.Request, "token", out var token) ||
                !application.Token.Equals(token, StringComparison.OrdinalIgnoreCase))
                context.Result = Error(ErrorCode.InvalidToken);
        }

        /// <summary>
        /// 是否匿名就可访问。
        /// </summary>
        public bool Anonymousable { get; set; }
    }
}