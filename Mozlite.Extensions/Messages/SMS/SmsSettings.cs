using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Messages.SMS
{
    /// <summary>
    /// 短信配置。
    /// </summary>
    [Table("core_SMS_Settings")]
    public class SmsSettings : ExtendBase, IIdObject
    {
        /// <summary>
        /// 最多发送次数。
        /// </summary>
        public const int MaxTimes = 5;

        /// <summary>
        /// 每批获取未发送短信的数量。
        /// </summary>
        public const int BatchSize = 100;

        /// <summary>
        /// 获取或设置唯一Id。
        /// </summary>
        [Identity]
        public int Id { get; set; }

        /// <summary>
        /// 发送器名称。
        /// </summary>
        [Size(36)]
        public string Client { get; set; }

        /// <summary>
        /// 应用程序Id。
        /// </summary>
        [Size(36)]
        public string AppId { get; set; }

        /// <summary>
        /// 应用程序密钥。
        /// </summary>
        [Size(128)]
        public string AppSecret { get; set; }

        /// <summary>
        /// 转换为子对象（拷贝对象实例）。
        /// </summary>
        /// <typeparam name="TSettings">配置类型。</typeparam>
        /// <returns>返回转换后的对象。</returns>
        public virtual TSettings As<TSettings>() where TSettings : SmsSettings, new()
        {
            var settings = new TSettings();
            foreach (var extendKey in ExtendKeys)
            {
                settings[extendKey] = this[extendKey];
            }

            settings.Client = Client;
            settings.Id = Id;
            settings.AppId = AppId;
            settings.AppSecret = AppSecret;
            return settings;
        }
    }
}