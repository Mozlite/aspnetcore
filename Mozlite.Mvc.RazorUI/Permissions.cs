using Mozlite.Extensions.Security.Permissions;

namespace Mozlite.Mvc.RazorUI
{
    /// <summary>
    /// 后台权限。
    /// </summary>
    public class Permissions : PermissionProvider
    {
        /// <summary>
        /// 初始化权限实例。
        /// </summary>
        protected override void Init()
        {
            Add("admin", "登录后台", "允许用户可以登录到后台!");
            Add("task", "后台服务", "允许管理后台服务相关操作!");
        }

        /// <summary>
        /// 登录后台。
        /// </summary>
        public const string Administrator = "core.admin";

        /// <summary>
        /// 后台服务。
        /// </summary>
        public const string Task = "core.task";
    }
}
