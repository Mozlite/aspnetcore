using Mozlite.Mvc.Properties;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// API配置。
    /// </summary>
    public class ApiSettings
    {
        /// <summary>
        /// 令牌过期天数。
        /// </summary>
        public int TokenExpired { get; set; } = 72;

        /// <summary>
        /// 扩展名称。
        /// </summary>
        public static readonly string EventType = Resources.Api_EventType;
    }
}