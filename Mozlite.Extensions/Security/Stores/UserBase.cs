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
        /// 获取或设置用户ID。
        /// </summary>
        [Identity]
        public int UserId { get; set; }

        /// <summary>
        /// 获取或设置用户名称。
        /// </summary>
        [Size(64)]
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
        public string PasswordHash { get; set; }

        /// <summary>
        /// 密码重置或修改生成的安全戳。
        /// </summary>
        [Size(36)]
        public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();

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
        public bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// 是否激活电话号码或邮件验证。
        /// </summary>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// 锁定截止UTC时间。
        /// </summary>
        public DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// 登陆错误达到失败次数，是否锁定账户。
        /// </summary>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// 登入失败次数。
        /// </summary>
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
        /// 头像。
        /// </summary>
        [Size(256)]
        public string Avatar { get; set; } = "/images/avatar.png";

        /// <summary>
        /// 显示角色Id。
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 显示角色名称。
        /// </summary>
        [Size(64)]
        public string RoleName { get; set; }

        /// <summary>
        /// 用户账户操作。
        /// </summary>
        public int Action { get; set; }

        /// <summary>
        /// 判断当前用户账户操作是否归零。
        /// </summary>
        /// <param name="action">当前操作码。</param>
        public void AttachActionProvider(int action)
        {
            if (Action == action)
                Action = 0;
        }

        /// <summary>
        /// 返回当前用户的用户名。
        /// </summary>
        public override string ToString()
        {
            return UserName;
        }
    }
}