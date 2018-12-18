using Mozlite.Extensions.Properties;
using Mozlite.Extensions.Security.Stores;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 默认角色。
    /// </summary>
    public class DefaultRole
    {
        /// <summary>
        /// 网站所有者，最高权限角色名称。
        /// </summary>
        public const string OwnerName = "SYS:OWNER";

        /// <summary>
        /// 网站所有者，最高权限角色。
        /// </summary>
        public static readonly DefaultRole Owner = new DefaultRole(Resources.Owner, OwnerName, int.MaxValue);

        /// <summary>
        /// 网站所有者，最低权限角色名称。
        /// </summary>
        public const string MemberName = "SYS:MEMBER";

        /// <summary>
        /// 网站所有者，最低权限角色。
        /// </summary>
        public static readonly DefaultRole Member = new DefaultRole(Resources.Member, MemberName, 0);

        private DefaultRole(string name, string normalizedName, int roleLevel)
        {
            Name = name;
            NormalizedName = normalizedName;
            RoleLevel = roleLevel;
        }

        /// <summary>
        /// 角色名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 用于比对的角色名称。
        /// </summary>
        public string NormalizedName { get; }

        /// <summary>
        /// 角色等级。
        /// </summary>
        public int RoleLevel { get; }

        /// <summary>
        /// 管理员角色值。
        /// </summary>
        public const int SystemRoleLevel = 2000000000;

        /// <summary>
        /// 转化为角色实例。
        /// </summary>
        /// <returns>返回当前角色实例。</returns>
        /// <typeparam name="TRole">角色类型。</typeparam>
        public TRole As<TRole>() where TRole : RoleBase, new()
        {
            return new TRole { Name = Name, NormalizedName = NormalizedName, RoleLevel = RoleLevel };
        }
    }
}