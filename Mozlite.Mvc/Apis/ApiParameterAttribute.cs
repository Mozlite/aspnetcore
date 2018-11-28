using System;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// 参数特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public class ApiParameterAttribute : Attribute
    {
        /// <summary>
        /// 描述信息。
        /// </summary>
        public string Description { get; }
        private readonly object _defaultValue;
        /// <summary>
        /// 初始化类<see cref="ApiParameterAttribute"/>。
        /// </summary>
        /// <param name="description">描述信息。</param>
        public ApiParameterAttribute(string description)
        {
            Description = description;
        }

        /// <summary>
        /// 初始化类<see cref="ApiParameterAttribute"/>。
        /// </summary>
        /// <param name="defaultValue">默认值。</param>
        protected ApiParameterAttribute(object defaultValue)
        {
            _defaultValue = defaultValue;
        }

        /// <summary>
        /// 获取当前实例返回的JSON字符串。
        /// </summary>
        /// <returns>当前实例返回的JSON字符串。</returns>
        public override string ToString()
        {
            return _defaultValue.ToJsonString();
        }
    }
}