using System.Collections.Generic;

namespace Mozlite.Extensions.Security.Permissions
{
    /// <summary>
    /// 权限提供者。
    /// </summary>
    public interface IPermissionProvider : IServices
    {
        /// <summary>
        /// 分类。
        /// </summary>
        string Category { get; }

        /// <summary>
        /// 排序。
        /// </summary>
        int Order { get; }

        /// <summary>
        /// 权限列表。
        /// </summary>
        /// <returns>返回权限列表。</returns>
        IEnumerable<Permission> LoadPermissions();
    }
}