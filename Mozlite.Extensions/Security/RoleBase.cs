using System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 角色基类。
    /// </summary>
    [Table("core_Roles")]
    public abstract class RoleBase : IdentityRole<int>
    {
        /// <summary>
        /// 角色Id。
        /// </summary>
        [Identity]
        public override int Id { get; set; }

        /// <summary>
        /// 角色名称。
        /// </summary>
        [Size(64)]
        public override string Name { get; set; }

        /// <summary>
        /// 用于比对的角色名称。
        /// </summary>
        [Size(64)]
        public override string NormalizedName { get; set; }

        /// <summary>
        /// 角色等级。
        /// </summary>
        [NotUpdated]
        public virtual int RoleLevel { get; set; }

        /// <summary>
        /// 随机值，每次更改的时候将改变。
        /// </summary>
        [Size(36)]
        public override string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 颜色。
        /// </summary>
        [Size(36)]
        public virtual string Color { get; set; }

        /// <summary>
        /// 图标。
        /// </summary>
        [Size(256)]
        public virtual string IconUrl { get; set; }

        /// <summary>
        /// 判断角色大小。
        /// </summary>
        /// <param name="role1">角色1。</param>
        /// <param name="role2">角色2。</param>
        /// <returns>返回判断结果。</returns>
        public static bool operator >=(RoleBase role1, RoleBase role2)
        {
            return role1?.RoleLevel >= role2?.RoleLevel;
        }

        /// <summary>
        /// 判断角色大小。
        /// </summary>
        /// <param name="role1">角色1。</param>
        /// <param name="role2">角色2。</param>
        /// <returns>返回判断结果。</returns>
        public static bool operator <=(RoleBase role1, RoleBase role2)
        {
            return role1?.RoleLevel <= role2?.RoleLevel;
        }

        /// <summary>
        /// 判断角色大小。
        /// </summary>
        /// <param name="role1">角色1。</param>
        /// <param name="role2">角色2。</param>
        /// <returns>返回判断结果。</returns>
        public static bool operator >(RoleBase role1, RoleBase role2)
        {
            return role1?.RoleLevel > role2?.RoleLevel;
        }

        /// <summary>
        /// 判断角色大小。
        /// </summary>
        /// <param name="role1">角色1。</param>
        /// <param name="role2">角色2。</param>
        /// <returns>返回判断结果。</returns>
        public static bool operator <(RoleBase role1, RoleBase role2)
        {
            return role1?.RoleLevel < role2?.RoleLevel;
        }

        /// <summary>
        /// 是否为系统角色。
        /// </summary>
        public bool IsSystem => RoleLevel == 0 || RoleLevel >= DefaultRole.SystemRoleLevel;

        /// <summary>
        /// 返回角色名称。
        /// </summary>
        public override string ToString()
        {
            return Name;
        }
    }
}