using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mozlite.Extensions.Security;

namespace Demo.Extensions.Security
{
    /// <summary>
    /// 用户管理。
    /// </summary>
    public class UserManager : UserManagerBase<User, Role, UserClaim, UserRole, UserLogin, UserToken, RoleClaim>, IUserManager
    {
        /// <summary>
        /// 初始化类<see cref="UserManager"/>。
        /// </summary>
        /// <param name="store">用户存储接口。</param>
        /// <param name="optionsAccessor"><see cref="T:Microsoft.AspNetCore.Identity.IdentityOptions" />实例对象。</param>
        /// <param name="passwordHasher">密码加密器接口。</param>
        /// <param name="userValidators">用户验证接口。</param>
        /// <param name="passwordValidators">密码验证接口。</param>
        /// <param name="keyNormalizer">唯一键格式化字符串。</param>
        /// <param name="errors">错误实例。</param>
        /// <param name="services">服务提供者接口。</param>
        /// <param name="logger">日志接口。</param>
        public UserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
    }
}