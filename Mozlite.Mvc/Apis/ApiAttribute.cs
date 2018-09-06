﻿using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// API特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ApiAttribute : RouteAttribute, IAuthorizationFilter
    {
        private const string AppId = "appid";

        private bool TryGetValue(HttpRequest request, string key, out StringValues value)
        {
            if (request.Headers.TryGetValue(key, out value) ||request.Query.TryGetValue(key, out value))
                return true;
            if (request.Method == "POST")
                return request.Form.TryGetValue(key, out value);
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
            context.HttpContext.Items[typeof(Application)] = application;
            if (Anonymousable)//无需验证
                return;
            if (application.ExpiredDate <= DateTime.Now)
            {
                context.Result = Error(ErrorCode.TokenExpired);
                return;
            }
            if (!TryGetValue(context.HttpContext.Request, "token", out var token) || !application.Token.Equals(token, StringComparison.OrdinalIgnoreCase))
            {
                context.Result = Error(ErrorCode.InvalidToken);
            }
        }

        /// <summary>
        /// 是否匿名就可访问。
        /// </summary>
        public bool Anonymousable { get; set; }

        /// <summary>
        /// 初始化类<see cref="ApiAttribute"/>。
        /// </summary>
        /// <param name="name">API名称。</param>
        public ApiAttribute(string name) : base($"openapis/{name}")
        {
        }
    }
}