using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Mozlite.Extensions.Security.Permissions
{
    /// <summary>
    /// 权限验证处理方法类。
    /// </summary>
    public class PermissionAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement>
    {
        private readonly IPermissionManager _permissionManager;
        /// <summary>
        /// 初始化类<see cref="PermissionAuthorizationHandler"/>。
        /// </summary>
        /// <param name="permissionManager">权限管理接口实例对象。</param>
        public PermissionAuthorizationHandler(IPermissionManager permissionManager)
        {
            _permissionManager = permissionManager;
        }

        /// <summary>
        /// 验证当前权限的合法性。
        /// </summary>
        /// <param name="context">验证上下文。</param>
        /// <param name="requirement">权限实例。</param>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
        {
            var permissionName = requirement?.Name;
            if (permissionName == null && context.Resource is ControllerActionDescriptor resource)
            {
                if (resource.RouteValues.TryGetValue("area", out var area))
                    permissionName = area + ".";
                permissionName += $"{resource.ControllerName}.{resource.ActionName}";
            }
            if (permissionName == null)
            {
                context.Fail();
                return;
            }
            if (await _permissionManager.IsAuthorizedAsync(permissionName))
                context.Succeed(requirement);
            else
                context.Fail();
        }
    }
}