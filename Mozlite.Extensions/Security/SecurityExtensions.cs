using System.Security.Claims;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 安全扩展类型。
    /// </summary>
    public static class SecurityExtensions
    {
        /// <summary>
        /// 判断当前用户是否符合要求。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <param name="userId">当前用户Id。</param>
        /// <returns>返回判断结果。</returns>
        public static bool IsCurrent(this ClaimsPrincipal user, int userId)
        {
            if (userId <= 0)
                return false;
            return user.IsInRole(IdentitySettings.Administrator) ||
                   user.FindFirstValue(ClaimTypes.NameIdentifier) ==
                   userId.ToString();
        }

        /// <summary>
        /// 判断当前用户是否位管理员。
        /// </summary>
        /// <param name="user">用户实例对象。</param>
        /// <returns>返回判断结果。</returns>
        public static bool IsAdministrator(this ClaimsPrincipal user)
        {
            return user.IsInRole(IdentitySettings.Administrator);
        }
    }
}