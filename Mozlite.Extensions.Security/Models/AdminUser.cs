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
        /// 用户ID。
        /// </summary>
        [Identity]
        public int UserId { get; set; }

        /// <summary>
        /// 用户名称。
        /// </summary>
        [Size(64)]
        [NotUpdated]
        public string UserName { get; set; }

        /// <summary>
        /// 昵称。
        /// </summary>
        [Size(64)]
        [NotUpdated]
        public string NickName { get; set; }

        /// <summary>
        /// 用于验证的用户名称。
        /// </summary>
        [Size(64)]
        [NotUpdated]
        public string NormalizedUserName { get; set; }

        /// <summary>
        /// 头像。
        /// </summary>
        [Size(256)]
        [NotUpdated]
        public string Avatar { get; set; }

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
        [Size(64)]
        [NotUpdated]
        public string SecurityStamp { get; set; }

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
        /// 是否锁定账户。
        /// </summary>
        [NotUpdated]
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// 登入失败次数。
        /// </summary>
        [NotUpdated]
        public int AccessFailedCount { get; set; }

        /// <summary>
        /// 注册IP。
        /// </summary>
        [Size(20)]
        [NotUpdated]
        public string CreatedIP { get; set; }

        /// <summary>
        /// 登入IP。
        /// </summary>
        [Size(20)]
        [NotUpdated]
        public string LoginIP { get; set; }

        /// <summary>
        /// 注册时间。
        /// </summary>
        [NotUpdated]
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// 更新时间。
        /// </summary>
        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// 最后登入时间。
        /// </summary>
        [NotUpdated]
        public DateTimeOffset? LastLoginDate { get; set; }

        /// <summary>
        /// 积分。
        /// </summary>
        public int Score { get; set; }
    }
}