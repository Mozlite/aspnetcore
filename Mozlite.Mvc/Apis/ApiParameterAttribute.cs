using System;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// 参数特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public class ApiParameterAttribute : Attribute
    {
        private readonly string _description;

        /// <summary>
        /// 初始化类<see cref="ApiParameterAttribute"/>。
        /// </summary>
        /// <param name="description">描述信息。</param>
        public ApiParameterAttribute(string description)
        {
            _description = description;
        }

        /// <summary>
        /// 初始化类<see cref="ApiParameterAttribute"/>。
        /// </summary>
        /// <param name="defaultValue">默认值。</param>
        protected ApiParameterAttribute(object defaultValue)
            : this(defaultValue?.ToJsonString())
        {
        }

        /// <summary>
        /// 获取当前实例返回的JSON字符串。
        /// </summary>
        /// <returns>当前实例返回的JSON字符串。</returns>
        public override string ToString()
        {
            return _description;
        }
    }
}