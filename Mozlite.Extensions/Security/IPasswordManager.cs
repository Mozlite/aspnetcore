namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 密码管理辅助接口。
    /// </summary>
    public interface IPasswordManager : ISingletonService
    {
        /// <summary>
        /// 加密字符串。
        /// </summary>
        /// <param name="password">原始密码。</param>
        /// <returns>返回加密后得字符串。</returns>
        string HashPassword(string password);

        /// <summary>
        /// 验证密码。
        /// </summary>
        /// <param name="hashedPassword">原始已经加密得密码。</param>
        /// <param name="providedPassword">要验证得原始密码。</param>
        /// <returns>验证结果。</returns>
        bool VerifyHashedPassword(string hashedPassword, string providedPassword);

        /// <summary>
        /// 拼接密码。
        /// </summary>
        /// <param name="userName">当前用户名。</param>
        /// <param name="password">密码。</param>
        /// <returns>返回拼接后得字符串。</returns>
        string PasswordSalt(string userName, string password);
    }
}