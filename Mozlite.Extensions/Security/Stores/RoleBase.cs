using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 角色基类。
    /// </summary>
    [Table("core_Roles")]
    public abstract class RoleBase
    {
        /// <summary>
        /// 角色Id。
        /// </summary>
        [Identity]
        public int RoleId { get; set; }

        /// <summary>
        /// 角色名称。
        /// </summary>
        [Size(64)]
        public string Name { get; set; }

        /// <summary>
        /// 用于比对的角色名称。
        /// </summary>
        [Size(64)]
        [NotUpdated]
        public string NormalizedName { get; set; }

        /// <summary>
        /// 角色等级。
        /// </summary>
        [NotUpdated]
        public int RoleLevel { get; set; }

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