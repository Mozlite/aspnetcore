using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Mozlite.Extensions.Security.Permissions
{
    /// <summary>
    /// 权限验证特性。
    /// </summary>
    public class PermissionAuthorizeAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// 初始化类<see cref="PermissionAuthorizeAttribute"/>。
        /// </summary>
        /// <param name="permission">当前权限名称。</param>
        public PermissionAuthorizeAttribute(string permission) : base(typeof(PermissionAuthorizeAttributeImpl))
        {
            Arguments = new object[] { new OperationAuthorizationRequirement { Name = permission } };
        }

        /// <summary>
        /// 初始化类<see cref="PermissionAuthorizeAttribute"/>。
        /// </summary>
        public PermissionAuthorizeAttribute() : base(typeof(PermissionAuthorizeAttributeImpl))
        {
        }

        private class PermissionAuthorizeAttributeImpl : Attribute, IAsyncAuthorizationFilter
        {
            private readonly IAuthorizationService _authorizationService;
            private readonly OperationAuthorizationRequirement _requirement;

            public PermissionAuthorizeAttributeImpl(IAuthorizationService authorizationService, OperationAuthorizationRequirement requirement)
            {
                _authorizationService = authorizationService;
                _requirement = requirement;
            }

            public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
            {
                if (!context.HttpContext.User.Identity.IsAuthenticated)
                {
                    context.Result = new ChallengeResult();
                    return;
                }
                var result = await _authorizationService.AuthorizeAsync(context.HttpContext.User,
                     context.ActionDescriptor, _requirement);
                if (result.Succeeded)
                    return;
                context.Result = new ForbidResult();
            }
        }
    }
}