using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 用户组基类。
    /// </summary>
    [Table("core_Roles")]
    public abstract class RoleBase
    {
        /// <summary>
        /// 用户组Id。
        /// </summary>
        [Identity]
        public int RoleId { get; set; }

        /// <summary>
        /// 用户组名称。
        /// </summary>
        [Size(64)]
        public string Name { get; set; }

        /// <summary>
        /// 用于比对的用户组名称。
        /// </summary>
        [Size(64)]
        [NotUpdated]
        public string NormalizedName { get; set; }

        /// <summary>
        /// 用户组等级。
        /// </summary>
        [NotUpdated]
        public int RoleLevel { get; set; }

        /// <summary>
        /// 判断用户组大小。
        /// </summary>
        /// <param name="role1">用户组1。</param>
        /// <param name="role2">用户组2。</param>
        /// <returns>返回判断结果。</returns>
        public static bool operator >=(RoleBase role1, RoleBase role2)
        {
            return role1?.RoleLevel >= role2?.RoleLevel;
        }

        /// <summary>
        /// 判断用户组大小。
        /// </summary>
        /// <param name="role1">用户组1。</param>
        /// <param name="role2">用户组2。</param>
        /// <returns>返回判断结果。</returns>
        public static bool operator <=(RoleBase role1, RoleBase role2)
        {
            return role1?.RoleLevel <= role2?.RoleLevel;
        }

        /// <summary>
        /// 判断用户组大小。
        /// </summary>
        /// <param name="role1">用户组1。</param>
        /// <param name="role2">用户组2。</param>
        /// <returns>返回判断结果。</returns>
        public static bool operator >(RoleBase role1, RoleBase role2)
        {
            return role1?.RoleLevel > role2?.RoleLevel;
        }

        /// <summary>
        /// 判断用户组大小。
        /// </summary>
        /// <param name="role1">用户组1。</param>
        /// <param name="role2">用户组2。</param>
        /// <returns>返回判断结果。</returns>
        public static bool operator <(RoleBase role1, RoleBase role2)
        {
            return role1?.RoleLevel < role2?.RoleLevel;
        }

        /// <summary>
        /// 是否为系统用户组。
        /// </summary>
        public bool IsSystem => RoleLevel == 0 || RoleLevel == int.MaxValue;

        /// <summary>
        /// 返回用户组名称。
        /// </summary>
        public override string ToString()
        {
            return Name;
        }
    }
}