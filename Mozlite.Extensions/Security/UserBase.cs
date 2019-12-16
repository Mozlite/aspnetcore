using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 用户基类。
    /// </summary>
    [Table("core_Users")]
    public abstract class UserBase : IdentityUser<int>
    {
        /// <summary>
        /// 获取或设置用户ID。
        /// </summary>
        [Identity]
        [PersonalData]
        public override int Id { get; set; }

        /// <summary>
        /// 获取或设置用户姓名。
        /// </summary>
        [Size(64)]
        [ProtectedPersonalData]
        public virtual string RealName { get; set; }

        /// <summary>
        /// 获取或设置用户名称。
        /// </summary>
        [NotUpdated]
        [Size(64)]
        [ProtectedPersonalData]
        public override string UserName { get; set; }

        /// <summary>
        /// 用于验证的用户名称。
        /// </summary>
        [NotUpdated]
        [Size(64)]
        public override string NormalizedUserName { get; set; }

        /// <summary>
        /// 电子邮件。
        /// </summary>
        [Size(256)]
        [ProtectedPersonalData]
        public override string Email { get; set; }

        /// <summary>
        /// 用于验证的电子邮件。
        /// </summary>
        [Size(256)]
        public override string NormalizedEmail { get; set; }

        /// <summary>
        /// 电子邮件是否已经确认过。
        /// </summary>
        [PersonalData]
        public override bool EmailConfirmed { get; set; }

        /// <summary>
        /// 加密后的密码。
        /// </summary>
        [NotUpdated]
        [Size(128)]
        public override string PasswordHash { get; set; }

        /// <summary>
        /// 密码重置或修改生成的安全戳。
        /// </summary>
        [Size(36)]
        public override string SecurityStamp { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 用于多线程更新附加随机条件。
        /// </summary>
        [Size(36)]
        public override string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 电话号码。
        /// </summary>
        [Size(20)]
        [ProtectedPersonalData]
        public override string PhoneNumber { get; set; }

        /// <summary>
        /// 是否已经验证电话号码。
        /// </summary>
        [PersonalData]
        public override bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// 是否激活电话号码或邮件验证。
        /// </summary>
        [NotUpdated]
        [PersonalData]
        public override bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// 锁定截止UTC时间。
        /// </summary>
        [NotUpdated]
        public override DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// 登录错误达到失败次数，是否锁定账户。
        /// </summary>
        [NotUpdated]
        public override bool LockoutEnabled { get; set; }

        /// <summary>
        /// 登录失败次数。
        /// </summary>
        [NotUpdated]
        public override int AccessFailedCount { get; set; }

        /// <summary>
        /// 注册IP。
        /// </summary>
        [Size(20)]
        [NotUpdated]
        public virtual string CreatedIP { get; set; }

        /// <summary>
        /// 登录IP。
        /// </summary>
        [Size(20)]
        [NotUpdated]
        public virtual string LoginIP { get; set; }

        /// <summary>
        /// 注册时间。
        /// </summary>
        [NotUpdated]
        public virtual DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// 更新时间。
        /// </summary>
        public virtual DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// 最后登录时间。
        /// </summary>
        [NotUpdated]
        public virtual DateTimeOffset? LastLoginDate { get; set; }

        /// <summary>
        /// 头像。
        /// </summary>
        [NotUpdated]
        [Size(256)]
        public virtual string Avatar { get; set; } = "/images/avatar.png";

        /// <summary>
        /// 显示角色Id。
        /// </summary>
        [NotUpdated]
        public virtual int RoleId { get; set; }

        /// <summary>
        /// 显示角色名称。
        /// </summary>
        [NotUpdated]
        [Size(64)]
        public virtual string RoleName { get; set; }

        /// <summary>
        /// 父级Id。
        /// </summary>
        [NotUpdated]
        public virtual int ParentId { get; set; }

        /// <summary>
        /// 返回当前用户的用户名。
        /// </summary>
        public override string ToString()
        {
            return UserName;
        }
    }
}