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
        public ApiDataResultAttribute(Type dataType)
            : this(Activator.CreateInstance(dataType))
        {

        }

        /// <summary>
        /// 初始化类<see cref="ApiDataResultAttribute"/>。
        /// </summary>
        /// <param name="defaultValue">默认值。</param>
        protected ApiDataResultAttribute(object defaultValue)
        {
            _defaultValue = defaultValue;
        }

        /// <summary>
        /// 获取实例。
        /// </summary>
        /// <returns>返回当前实例。</returns>
        protected override ApiResult Init() => new ApiDataResult(_defaultValue);
    }
}