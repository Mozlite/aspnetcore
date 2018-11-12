using Mozlite.Extensions.Security.Permissions;

namespace MS.Areas.Security
{
    /// <summary>
    /// 权限。
    /// </summary>
    public class Permissions : PermissionProvider
    {
        /// <summary>
        /// 分类。
        /// </summary>
        public override string Category => "users";

        /// <summary>
        /// 初始化权限实例。
        /// </summary>
        protected override void Init()
        {
            Add("manager", "管理用户", "对用户账户进行管理操作！");
            Add("roles", "管理角色", "对角色进行管理操作！");
            Add("permissions", "权限管理", "对所有权限进行管理操作！");
            Add("logs", "日志管理", "管理用户操作日志！");
            Add("settings", "用户配置", "管理用户配置权限！");
        }

        /// <summary>
        /// 管理用户权限。
        /// </summary>
        public const string Users = "users.manager";

        /// <summary>
        /// 管理用户组权限。
        /// </summary>
        public const string Roles = "users.roles";

        /// <summary>
        /// 权限管理权限。
        /// </summary>
        public const string PermissionManager = "users.permissions";

        /// <summary>
        /// 管理日志权限。
        /// </summary>
        public const string Logs = "users.logs";

        /// <summary>
        /// 用户配置权限。
        /// </summary>
        public const string Settings = "users.settings";
    }
}