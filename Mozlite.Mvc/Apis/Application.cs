using Mozlite.Extensions;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// 应用程序。
    /// </summary>
    [Table("apis_Applications")]
    public class Application : ExtendBase
    {
        /// <summary>
        /// 应用程序Id。
        /// </summary>
        [Identity]
        public int Id { get; set; }

        /// <summary>
        /// 应用程序Id。
        /// </summary>
        public Guid AppId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 应用程序名称。
        /// </summary>
        [Size(32)]
        public string Name { get; set; }

        /// <summary>
        /// 应用程序名称。
        /// </summary>
        [Size(256)]
        public string Description { get; set; }

        /// <summary>
        /// 应用程序密钥(128位随机字符串)。
        /// </summary>
        [Size(32)]
        public string AppSecret { get; set; } = Cores.GeneralKey(128);

        /// <summary>
        /// 令牌字符串。
        /// </summary>
        [Size(32)]
        [NotUpdated]
        public string Token { get; set; }

        /// <summary>
        /// 过期时间。
        /// </summary>
        [NotUpdated]
        public DateTimeOffset ExpiredDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// 添加时间。
        /// </summary>
        [NotUpdated]
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// 扩展名称。
        /// </summary>
        [Size(32)]
        public string ExtensionName { get; set; }

        /// <summary>
        /// 关联Id。
        /// </summary>
        public int? TargetId { get; set; }
        
        private string[] _disabled;
        /// <summary>
        /// 禁用的API名称。
        /// </summary>
        [NotMapped]
        public string[] Disabled
        {
            get
            {
                if (_disabled == null)
                {
                    var disabled = this[nameof(Disabled)];
                    _disabled = disabled == null ? new string[0] : disabled.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }

                return _disabled;
            }
            set
            {
                _disabled = value;
                this[nameof(Disabled)] = string.Join(",", value);
            }
        }
    }
}