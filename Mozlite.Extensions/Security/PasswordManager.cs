using Microsoft.AspNetCore.Identity;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 密码管理实现类。
    /// </summary>
    public abstract class PasswordManager<TUser> : IPasswordManager
        where TUser : class
    {
        private readonly ILookupNormalizer _normalizer;
        private readonly IPasswordHasher<TUser> _passwordHasher;
        /// <summary>
        /// 初始化类<see cref="PasswordManager{TUser}"/>。
        /// </summary>
        /// <param name="normalizer">格式化键接口。</param>
        /// <param name="passwordHasher">密码加密器。</param>
        protected PasswordManager(ILookupNormalizer normalizer, IPasswordHasher<TUser> passwordHasher)
        {
            _normalizer = normalizer;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// 加密字符串。
        /// </summary>
        /// <param name="password">原始密码。</param>
        /// <returns>返回加密后得字符串。</returns>
        public virtual string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(null, password);
        }

        /// <summary>
        /// 验证密码。
        /// </summary>
        /// <param name="hashedPassword">原始已经加密得密码。</param>
        /// <param name="providedPassword">要验证得原始密码。</param>
        /// <returns>验证结果。</returns>
        public virtual bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            return _passwordHasher.VerifyHashedPassword(null, hashedPassword, providedPassword) !=
                   PasswordVerificationResult.Failed;
        }

        /// <summary>
        /// 拼接密码。
        /// </summary>
        /// <param name="userName">当前用户名。</param>
        /// <param name="password">密码。</param>
        /// <returns>返回拼接后得字符串。</returns>
        public virtual string PasswordSalt(string userName, string password)
        {
            return $"{NormalizeKey(userName)}2018{password}";
        }

        /// <summary>
        /// 正常实例化键。
        /// </summary>
        /// <param name="key">原有键值。</param>
        /// <returns>返回正常化后的字符串。</returns>
        protected string NormalizeKey(string key)
        {
            if (_normalizer != null)
                return _normalizer.Normalize(key);
            return key;
        }
    }
}