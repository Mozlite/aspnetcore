using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Mozlite.Extensions.Security;

namespace Mozlite.Extensions
{
    /// <summary>
    /// 扩展类。
    /// </summary>
    public static class MvcExtensions
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
            return claims.FindFirstValue(ClaimTypes.Name) ?? IdentitySettings.Anonymous;
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
    }
}
