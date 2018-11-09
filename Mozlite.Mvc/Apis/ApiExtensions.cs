using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// API扩展类。
    /// </summary>
    public static class ApiExtensions
    {
        private static readonly ApiResultAttribute DefaultResult = new ApiResultAttribute();
        /// <summary>
        /// 获取默认结果。
        /// </summary>
        /// <param name="api">当前API描述信息。</param>
        /// <returns>返回默认值特性。</returns>
        public static ApiResultAttribute GetDefaultResult(this ApiDescription api)
        {
            return (api.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo.GetCustomAttribute<ApiResultAttribute>() ??
                 DefaultResult;
        }
    }
}