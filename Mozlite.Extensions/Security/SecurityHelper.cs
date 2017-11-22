namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 辅助类。
    /// </summary>
    public static class SecurityHelper
    {
        /// <summary>
        /// 拼接用户密码。
        /// </summary>
        /// <param name="userName">用户名称。</param>
        /// <param name="password">原始密码。</param>
        /// <returns>返回拼接后的密码。</returns>
        public static string CreatePassword(string userName, string password)
        {
            return userName.ToUpper() + "m:z" + password;
        }
    }
}