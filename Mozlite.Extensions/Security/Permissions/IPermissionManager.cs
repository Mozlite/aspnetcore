using System.Threading.Tasks;

namespace Mozlite.Extensions.Security.Permissions
{
    /// <summary>
    /// 权限管理接口。
    /// </summary>
    public interface IPermissionManager : ISingletonService
    {
        /// <summary>
        /// 获取当前用户的权限。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="permissioName">权限名称。</param>
        /// <returns>返回权限结果。</returns>
        Task<PermissionValue> GetPermissionAsync(int userId, string permissioName);

        /// <summary>
        /// 判断当前用户是否拥有<paramref name="permissionName"/>权限。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        /// <returns>返回判断结果。</returns>
        Task<bool> IsAuthorized(string permissionName);
    }
}