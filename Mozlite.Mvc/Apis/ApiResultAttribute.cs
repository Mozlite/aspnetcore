using Newtonsoft.Json;
using System;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// 输出特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ApiResultAttribute : Attribute
    {
        /// <summary>
        /// 获取当前实例返回的JSON字符串。
        /// </summary>
        /// <returns>当前实例返回的JSON字符串。</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(Init(), _settings);
        }

        private static readonly ApiResult _result = new ApiResult(ErrorCode.Succeeded);

        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
            NullValueHandling = NullValueHandling.Ignore
        };

        /// <summary>
        /// 获取实例。
        /// </summary>
        /// <returns>返回当前实例。</returns>
        protected virtual ApiResult Init() => _result;
    }
}