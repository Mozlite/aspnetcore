using System.ComponentModel;

namespace MS.Extensions.Security
{
    /// <summary>
    /// 安全配置。
    /// </summary>
    public class SecuritySettings
    {
        /// <summary>
        /// 扩展名称（区域名称）。
        /// </summary>
        public const string ExtensionName = "security";

        /// <summary>
        /// 事件ID。
        /// </summary>
        public const int EventId = 1;

        /// <summary>
        /// 最大事件ID，事件ID定义1,2,3,4...主要用于下拉列表资源定义。
        /// </summary>
        public const int MaxEventId = 1;//todo:修改最大事件ID根据项目定义

        /// <summary>
        /// 是否需要确认电子邮件。
        /// </summary>
        [DisplayName("确认电子邮件")]
        public bool RequiredEmailConfirmed { get; set; }

        /// <summary>
        /// 是否需要确认电话号码。
        /// </summary>
        [DisplayName("确认电话号码")]
        public bool RequiredPhoneNumberConfirmed { get; set; }

        /// <summary>
        /// 是否需要二次验证。
        /// </summary>
        [DisplayName("二次登陆验证")]
        public bool RequiredTwoFactorEnabled { get; set; }

        /// <summary>
        /// 开放注册。
        /// </summary>
        [DisplayName("开放注册")]
        public bool Registrable { get; set; }
    }
}