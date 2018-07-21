using Mozlite.Extensions.Properties;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 默认用户组。
    /// </summary>
    public class DefaultRole
    {
        /// <summary>
        /// 网站所有者，最高权限用户组。
        /// </summary>
        public static readonly DefaultRole Owner = new DefaultRole(Resources.Owner, "SYS:OWNER", int.MaxValue);

        /// <summary>
        /// 网站所有者，最低权限用户组。
        /// </summary>
        public static readonly DefaultRole Member = new DefaultRole(Resources.Member, "SYS:MEMBER", 0);

        private DefaultRole(string name, string normalizedName, int roleLevel)
        {
            Name = name;
            NormalizedName = normalizedName;
            RoleLevel = roleLevel;
        }

        /// <summary>
        /// 用户组名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 用于比对的用户组名称。
        /// </summary>
        public string NormalizedName { get; }

        /// <summary>
        /// 用户组等级。
        /// </summary>
        public int RoleLevel { get; }
    }
}