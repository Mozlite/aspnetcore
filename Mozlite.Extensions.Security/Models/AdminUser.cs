using System;

namespace Mozlite.Extensions.Security.Models
{
    /// <summary>
    /// 管理员用户，用户添加管理员。
    /// </summary>
    [Target(typeof(User))]
    public class AdminUser
    {
        /// <summary>
        /// 用户名称。
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 昵称。
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 用于验证的用户名称。
        /// </summary>
        public string NormalizedUserName { get; set; }

        /// <summary>
        /// 头像。
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 电子邮件。
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 用于验证的电子邮件。
        /// </summary>
        public string NormalizedEmail { get; set; }

        /// <summary>
        /// 电子邮件是否已经确认过。
        /// </summary>
        public bool EmailConfirmed { get; set; } = true;

        /// <summary>
        /// 加密后的密码。
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// 密码重置或修改生成的安全戳。
        /// </summary>
        public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 电话号码。
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 是否已经验证电话号码。
        /// </summary>
        public bool PhoneNumberConfirmed { get; set; } = true;

        /// <summary>
        /// 是否激活电话号码或邮件验证。
        /// </summary>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// 是否锁定账户。
        /// </summary>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// 登入失败次数。
        /// </summary>
        public int AccessFailedCount { get; set; }

        /// <summary>
        /// 注册时间。
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// 更新时间。
        /// </summary>
        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// 积分。
        /// </summary>
        public int Score { get; set; }
    }
}