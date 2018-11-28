using System;
using System.ComponentModel;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// 输出特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ApiResultAttribute : DescriptionAttribute
    {
        /// <summary>
        /// 获取当前实例返回的JSON字符串。
        /// </summary>
        /// <returns>当前实例返回的JSON字符串。</returns>
        public override string ToString()
        {
            return _result.ToJsonString();
        }

        private static readonly ApiResult _result = new ApiResult(ErrorCode.Succeeded);

        /// <summary>
        /// 初始化类<see cref="ApiResultAttribute"/>。
        /// </summary>
        public ApiResultAttribute()
        {
        }

        /// <summary>
        /// 初始化类<see cref="ApiResultAttribute"/>。
        /// </summary>
        /// <param name="description">描述信息。</param>
        public ApiResultAttribute(string description)
            : base(description)
        {
        }
    }
}