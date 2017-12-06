namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 用户配置。
    /// </summary>
    public class SecuritySettings : IdentitySettings
    {
        /// <summary>
        /// 扩展名称。
        /// </summary>
        public const string ExtensionName = "security";

        /// <summary>
        /// 默认头像。
        /// </summary>
        public string DefaultAvatar { get; set; }

        /// <summary>
        /// 事件Id。
        /// </summary>
        public const int EventId = 1;

        /// <summary>
        /// 媒体文件夹。
        /// </summary>
        public const int MediaDir = 1;
    }
}