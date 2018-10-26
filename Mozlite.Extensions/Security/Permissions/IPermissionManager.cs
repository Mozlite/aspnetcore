using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mozlite.Extensions.Security.Permissions
{
    /// <summary>
    /// 权限管理接口。
    /// </summary>
    public interface IPermissionManager : ISingletonService
    {
        /// <summary>
        /// 获取权限值。
        /// </summary>
        /// <param name="roleId">当前角色。</param>
        /// <param name="permissionId">权限Id。</param>
        /// <returns>返回权限值。</returns>
        PermissionValue GetPermissionValue(int roleId, int permissionId);

        /// <summary>
        /// 获取权限值。
        /// </summary>
        /// <param name="roleId">当前角色。</param>
        /// <param name="permissionId">权限Id。</param>
        /// <returns>返回权限值。</returns>
        Task<PermissionValue> GetPermissionValueAsync(int roleId, int permissionId);

        /// <summary>
        /// 获取当前用户的权限。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="permissioName">权限名称。</param>
        /// <returns>返回权限结果。</returns>
        PermissionValue GetUserPermissionValue(int userId, string permissioName);

        /// <summary>
        /// 获取当前用户的权限。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="permissioName">权限名称。</param>
        /// <returns>返回权限结果。</returns>
        Task<PermissionValue> GetUserPermissionValueAsync(int userId, string permissioName);

        /// <summary>
        /// 判断当前用户是否拥有<paramref name="permissionName"/>权限。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        /// <returns>返回判断结果。</returns>
        Task<bool> IsAuthorizedAsync(string permissionName);

        /// <summary>
        /// 判断当前用户是否拥有<paramref name="permissionName"/>权限。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        /// <returns>返回判断结果。</returns>
        bool IsAuthorized(string permissionName);

        /// <summary>
        /// 获取或添加权限。
        /// </summary>
        /// <param name="id">权限Id。</param>
        /// <returns>返回当前名称的权限实例。</returns>
        Permission GetPermission(int id);

        /// <summary>
        /// 获取或添加权限。
        /// </summary>
        /// <param name="id">权限Id。</param>
        /// <returns>返回当前名称的权限实例。</returns>
        Task<Permission> GetPermissionAsync(int id);

        /// <summary>
        /// 获取或添加权限。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        /// <returns>返回当前名称的权限实例。</returns>
        Permission GetOrCreate(string permissionName);

        /// <summary>
        /// 获取或添加权限。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        /// <returns>返回当前名称的权限实例。</returns>
        Task<Permission> GetOrCreateAsync(string permissionName);

        /// <summary>
        /// 保存权限。
        /// </summary>
        /// <param name="permission">权限实例对象。</param>
        /// <returns>返回保存结果。</returns>
        Task<bool> SaveAsync(Permission permission);

        /// <summary>
        /// 保存权限。
        /// </summary>
        /// <param name="permission">权限实例对象。</param>
        /// <returns>返回保存结果。</returns>
        bool Save(Permission permission);

        /// <summary>
        /// 更新管理员权限配置。
        /// </summary>
        Task RefreshOwnersAsync();

        /// <summary>
        /// 更新管理员权限配置。
        /// </summary>
        /// <returns>返回更新结果。</returns>
        void RefreshOwners();

        /// <summary>
        /// 加载权限列表。
        /// </summary>
        /// <param name="category">权限分类。</param>
        /// <returns>返回权限列表。</returns>
        IEnumerable<Permission> LoadPermissions(string category = null);

        /// <summary>
        /// 加载权限列表。
        /// </summary>
        /// <param name="category">权限分类。</param>
        /// <returns>返回权限列表。</returns>
        Task<IEnumerable<Permission>> LoadPermissionsAsync(string category = null);

        /// <summary>
        /// 保存当前配置角色权限。
        /// </summary>
        /// <param name="roleId">角色Id。</param>
        /// <param name="request">当前请求。</param>
        /// <returns>返回保存结果。</returns>
        Task<DataResult> SaveAsync(int roleId, HttpRequest request);

        /// <summary>
        /// 保存当前配置角色权限。
        /// </summary>
        /// <param name="roleId">角色Id。</param>
        /// <param name="request">当前请求。</param>
        /// <returns>返回保存结果。</returns>
        DataResult Save(int roleId, HttpRequest request);

        /// <summary>
        /// 上移权限。
        /// </summary>
        /// <param name="id">权限Id。</param>
        /// <param name="category">分类。</param>
        /// <returns>返回移动结果。</returns>
        bool MoveUp(int id, string category);

        /// <summary>
        /// 上移权限。
        /// </summary>
        /// <param name="id">权限Id。</param>
        /// <param name="category">分类。</param>
        /// <returns>返回移动结果。</returns>
        Task<bool> MoveUpAsync(int id, string category);

        /// <summary>
        /// 下移权限。
        /// </summary>
        /// <param name="id">权限Id。</param>
        /// <param name="category">分类。</param>
        /// <returns>返回移动结果。</returns>
        bool MoveDown(int id, string category);

        /// <summary>
        /// 下移权限。
        /// </summary>
        /// <param name="id">权限Id。</param>
        /// <param name="category">分类。</param>
        /// <returns>返回移动结果。</returns>
        Task<bool> MoveDownAsync(int id, string category);

        /// <summary>
        /// 判断权限名称是否存在。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        /// <returns>返回判断结果。</returns>
        bool Exist(string permissionName);

        /// <summary>
        /// 判断权限名称是否存在。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        /// <returns>返回判断结果。</returns>
        Task<bool> ExistAsync(string permissionName);
    }
}