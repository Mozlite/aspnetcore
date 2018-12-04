using System;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// 输出特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ApiDataResultAttribute : ApiResultAttribute
    {
        private readonly object _defaultValue;

        /// <summary>
        /// 初始化类<see cref="ApiDataResultAttribute"/>。
        /// </summary>
        /// <param name="dataType">数据类型。</param>
        /// <param name="description">描述。</param>
        public ApiDataResultAttribute(Type dataType, string description)
            : this(Activator.CreateInstance(dataType), description)
        {

        }

        /// <summary>
        /// 初始化类<see cref="ApiDataResultAttribute"/>。
        /// </summary>
        /// <param name="defaultValue">默认值。</param>
        /// <param name="description">描述。</param>
        protected ApiDataResultAttribute(object defaultValue, string description)
            : base(description)
        {
            _defaultValue = defaultValue;
        }

        /// <summary>
        /// 获取当前实例返回的JSON字符串。
        /// </summary>
        /// <returns>当前实例返回的JSON字符串。</returns>
        public override string ToString()
        {
            return new ApiDataResult(_defaultValue).ToJsonString();
        }
    }
}