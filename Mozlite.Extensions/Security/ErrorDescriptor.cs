namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 错误枚举。
    /// </summary>
    public enum ErrorDescriptor
    {
        /// <summary>
        /// 发生了错误。
        /// </summary>
        DefaultError = DataAction.UnknownError,
        /// <summary>
        /// 并发处理错误，对象已经被更改。
        /// </summary>
        ConcurrencyFailure,
        /// <summary>
        /// 电子邮件已经存在。
        /// </summary>
        DuplicateEmail,
        /// <summary>
        /// 角色名称已经存在。
        /// </summary>
        DuplicateRoleName,
        /// <summary>
        /// 用户名已经存在。
        /// </summary>
        DuplicateUserName,
        /// <summary>
        /// 电子邮件无效。
        /// </summary>
        InvalidEmail,
        /// <summary>
        /// 用户被禁用。
        /// </summary>
        UserLockedOut,
        /// <summary>
        /// 角色名称无效。
        /// </summary>
        InvalidRoleName,
        /// <summary>
        /// 标识无效。
        /// </summary>
        InvalidToken,
        /// <summary>
        /// 用户没有在角色中。
        /// </summary>
        UserNotInRole,
        /// <summary>
        /// 用户锁定状态没有激活。
        /// </summary>
        UserLockoutNotEnabled,
        /// <summary>
        /// 用户已经包含在角色中。
        /// </summary>
        UserAlreadyInRole,
        /// <summary>
        /// 用户已经包含了密码。
        /// </summary>
        UserAlreadyHasPassword,
        /// <summary>
        /// 赎回兑换码失败。
        /// </summary>
        RecoveryCodeRedemptionFailed,
        /// <summary>
        /// 密码太短。
        /// </summary>
        PasswordTooShort,
        /// <summary>
        /// 密码需要大写字母。
        /// </summary>
        PasswordRequiresUpper,
        /// <summary>
        /// 密码字符不能重复。
        /// </summary>
        PasswordRequiresUniqueChars,
        /// <summary>
        /// 密码需要包含标点字符。
        /// </summary>
        PasswordRequiresNonAlphanumeric,
        /// <summary>
        /// 密码需要小写字母。
        /// </summary>
        PasswordRequiresLower,
        /// <summary>
        /// 密码需要包含数字。
        /// </summary>
        PasswordRequiresDigit,
        /// <summary>
        /// 密码错误。
        /// </summary>
        PasswordMismatch,
        /// <summary>
        /// 登陆已经关联。
        /// </summary>
        LoginAlreadyAssociated,
        /// <summary>
        /// 用户名无效。
        /// </summary>
        InvalidUserName,
        /// <summary>
        /// 用户不存在。
        /// </summary>
        UserNotFound,
        /// <summary>
        /// 角色唯一键已经存在。
        /// </summary>
        DuplicateNormalizedRoleName,
    }
}