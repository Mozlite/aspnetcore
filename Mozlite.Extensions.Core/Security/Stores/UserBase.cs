using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 用户基类。
    /// </summary>
    [Table("core_Users")]
    public abstract class UserBase
    {
        /// <summary>
        /// 初始化类<see cref="UserBase"/>。
        /// </summary>
        protected UserBase()
        {
        }

        /// <summary>
        /// 初始化类<see cref="UserBase"/>。
        /// </summary>
        /// <param name="userName">用户名称。</param>
        protected UserBase(string userName)
        {
            UserName = userName;
        }

        /// <summary>
        /// 获取或设置用户ID。
        /// </summary>
        [Identity]
        public int UserId { get; set; }

        /// <summary>
        /// 获取或设置用户名称。
        /// </summary>
        [Size(64)]
        [NotUpdated]
        public string UserName { get; set; }

        /// <summary>
        /// 用于验证的用户名称。
        /// </summary>
        [Size(64)]
        [NotUpdated]
        public string NormalizedUserName { get; set; }
        
        /// <summary>
        /// 电子邮件。
        /// </summary>
        [Size(256)]
        public string Email { get; set; }

        /// <summary>
        /// 用于验证的电子邮件。
        /// </summary>
        [Size(256)]
        public string NormalizedEmail { get; set; }

        /// <summary>
        /// 电子邮件是否已经确认过。
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// 加密后的密码。
        /// </summary>
        [Size(128)]
        [NotUpdated]
        public string PasswordHash { get; set; }

        /// <summary>
        /// 密码重置或修改生成的安全戳。
        /// </summary>
        [Size(36)]
        [NotUpdated]
        public string SecurityStamp { get; set; }

        /// <summary>
        /// 用于多线程更新附加随机条件。
        /// </summary>
        [Size(36)]
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 电话号码。
        /// </summary>
        [Size(20)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 是否已经验证电话号码。
        /// </summary>
        [NotUpdated]
        public bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// 是否激活电话号码或邮件验证。
        /// </summary>
        [NotUpdated]
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// 锁定截止UTC时间。
        /// </summary>
        [NotUpdated]
        public DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// 登陆错误达到失败次数，是否锁定账户。
        /// </summary>
        [NotUpdated]
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// 登入失败次数。
        /// </summary>
        [NotUpdated]
        public int AccessFailedCount { get; set; }

        /// <summary>
        /// 返回当前用户的用户名。
        /// </summary>
        public override string ToString()
        {
            return UserName;
        }
    }
}