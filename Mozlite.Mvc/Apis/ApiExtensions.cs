using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.ComponentModel;
using System.Reflection;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// API扩展类。
    /// </summary>
    public static class ApiExtensions
    {
        private static readonly ApiResultAttribute _defaultResult = new ApiResultAttribute();
        /// <summary>
        /// 获取默认结果。
        /// </summary>
        /// <param name="api">当前API描述信息。</param>
        /// <returns>返回默认值特性。</returns>
        public static ApiResultAttribute GetResult(this ApiDescription api)
        {
            return (api.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo.GetCustomAttribute<ApiResultAttribute>() ??
                 _defaultResult;
        }

        /// <summary>
        /// 获取描述信息。
        /// </summary>
        /// <param name="api">当前API描述信息。</param>
        /// <returns>返回API描述。</returns>
        public static string GetDescription(this ApiDescription api)
        {
            return (api.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
        }

        /// <summary>
        /// 获取描述信息。
        /// </summary>
        /// <param name="api">当前API描述信息。</param>
        /// <returns>返回API描述。</returns>
        public static string GetDescription(this ApiParameterDescription api)
        {
            return (api.ParameterDescriptor as ControllerParameterDescriptor)?.ParameterInfo.GetCustomAttribute<ApiParameterAttribute>()?.ToString();
        }

        /// <summary>
        /// 获取描述信息。
        /// </summary>
        /// <param name="api">当前API描述信息。</param>
        /// <returns>返回API描述。</returns>
        public static string GetParameter(this ApiDescription api)
        {
            return (api.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo.GetCustomAttribute<ApiParameterAttribute>()?.ToString();
        }
    }
}