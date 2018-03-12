using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 用户扩展类。
    /// </summary>
    public static class SecurityExtensions
    {
        /// <summary>
        /// 获取用户的IP地址。
        /// </summary>
        /// <param name="httpContext">当前HTTP上下文。</param>
        /// <returns>返回当前用户IP地址。</returns>
        public static string GetUserAddress(this HttpContext httpContext)
        {
            var ipAddress = httpContext.Connection?.RemoteIpAddress?.ToString();
            if (ipAddress != null)
                return ipAddress;
            var xff = httpContext.Request.Headers["x-forwarded-for"];
            if (xff.Count > 0)
            {
                ipAddress = xff.FirstOrDefault();
                return ipAddress?.Split(':').FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// 获取当前登入用户的Id。
        /// </summary>
        /// <param name="claims">当前用户接口实例。</param>
        /// <returns>返回用户Id，如果未登入则返回0。</returns>
        public static int GetUserId(this ClaimsPrincipal claims)
        {
            int.TryParse(claims.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);
            return userId;
        }

        /// <summary>
        /// 获取当前用户的用户名称。
        /// </summary>
        /// <param name="claims">当前用户接口实例。</param>
        /// <returns>返回用户名称，如果未登入则返回“Anonymous”。</returns>
        public static string GetUserName(this ClaimsPrincipal claims)
        {
            return claims.FindFirstValue(ClaimTypes.Name);
        }

        /// <summary>
        /// 获取当前用户的用户名称。
        /// </summary>
        /// <param name="claims">当前用户接口实例。</param>
        /// <returns>返回用户名称，如果未登入则返回“Anonymous”。</returns>
        public static string GetRoleName(this ClaimsPrincipal claims)
        {
            return claims.FindFirstValue(ClaimTypes.Role);
        }

        /// <summary>
        /// 获取安全错误。
        /// </summary>
        /// <param name="describer">错误描述实例。</param>
        /// <param name="errorDescriptor">错误枚举实例。</param>
        /// <param name="args">参数。</param>
        /// <returns>返回错误实例。</returns>
        public static IdentityError SecurityError(this IdentityErrorDescriber describer, ErrorDescriptor errorDescriptor, params object[] args)
        {
            if (describer is SecurityErrorDescriptor descriptor)
                return descriptor.Error(errorDescriptor, args);
            return describer.DefaultError();
        }

        /// <summary>
        /// 用户被锁定。
        /// </summary>
        /// <param name="describer">错误描述实例。</param>
        /// <returns>返回错误实例。</returns>
        public static IdentityError UserLockedOut(this IdentityErrorDescriber describer)
        {
            return describer.SecurityError(ErrorDescriptor.UserLockedOut);
        }

        /// <summary>
        /// 用户不存在。
        /// </summary>
        /// <param name="describer">错误描述实例。</param>
        /// <returns>返回错误实例。</returns>
        public static IdentityError UserNotFound(this IdentityErrorDescriber describer)
        {
            return describer.SecurityError(ErrorDescriptor.UserNotFound);
        }

        /// <summary>
        /// 角色唯一键已经存在。
        /// </summary>
        /// <param name="describer">错误描述实例。</param>
        /// <param name="normalizedRoleName">角色唯一键。</param>
        /// <returns>返回错误实例。</returns>
        public static IdentityError DuplicateNormalizedRoleName(this IdentityErrorDescriber describer, string normalizedRoleName)
        {
            return describer.SecurityError(ErrorDescriptor.DuplicateNormalizedRoleName, normalizedRoleName);
        }
    }
}