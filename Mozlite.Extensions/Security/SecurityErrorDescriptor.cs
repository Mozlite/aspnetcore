using Microsoft.AspNetCore.Identity;
using Mozlite.Mvc;

namespace Mozlite.Extensions.Security
{
    /// <summary>
    /// 错误描述。
    /// </summary>
    public class SecurityErrorDescriptor : IdentityErrorDescriber
    {
        private readonly ILocalizer _localizer;
        /// <summary>
        /// 初始化类<see cref="SecurityErrorDescriptor"/>。
        /// </summary>
        /// <param name="localizer">本地化接口。</param>
        public SecurityErrorDescriptor(ILocalizer localizer)
        {
            _localizer = localizer;
        }

        private IdentityError Error(IdentityError error, ErrorDescriptor descriptor, params object[] args)
        {
            error.Description = _localizer.GetString(descriptor, args);
            return error;
        }

        internal IdentityError Error(ErrorDescriptor descriptor, params object[] args)
        {
            return new IdentityError
            {
                Code = descriptor.ToString(),
                Description = _localizer.GetString(descriptor, args)
            };
        }

        /// <summary>
        /// Returns an <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a concurrency failure.
        /// </summary>
        /// <returns>An <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a concurrency failure.</returns>
        public override IdentityError ConcurrencyFailure()
        {
            return Error(base.ConcurrencyFailure(), ErrorDescriptor.ConcurrencyFailure);
        }

        /// <summary>
        /// Returns the default <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" />.
        /// </summary>
        /// <returns>The default <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" />.</returns>
        public override IdentityError DefaultError()
        {
            return Error(base.DefaultError(), ErrorDescriptor.DefaultError);
        }

        /// <summary>
        /// Returns an <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating the specified <paramref name="email" /> is already associated with an account.
        /// </summary>
        /// <param name="email">The email that is already associated with an account.</param>
        /// <returns>An <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating the specified <paramref name="email" /> is already associated with an account.</returns>
        public override IdentityError DuplicateEmail(string email)
        {
            return Error(base.DuplicateEmail(email), ErrorDescriptor.DuplicateEmail, email);
        }

        /// <summary>
        /// Returns an <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating the specified <paramref name="role" /> name already exists.
        /// </summary>
        /// <param name="role">The duplicate role.</param>
        /// <returns>An <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating the specific role <paramref name="role" /> name already exists.</returns>
        public override IdentityError DuplicateRoleName(string role)
        {
            return Error(base.DuplicateRoleName(role), ErrorDescriptor.DuplicateRoleName, role);
        }

        /// <summary>
        /// Returns an <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating the specified <paramref name="userName" /> already exists.
        /// </summary>
        /// <param name="userName">The user name that already exists.</param>
        /// <returns>An <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating the specified <paramref name="userName" /> already exists.</returns>
        public override IdentityError DuplicateUserName(string userName)
        {
            return Error(base.DuplicateUserName(userName), ErrorDescriptor.DuplicateUserName, userName);
        }

        /// <summary>
        /// Returns an <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating the specified <paramref name="email" /> is invalid.
        /// </summary>
        /// <param name="email">The email that is invalid.</param>
        /// <returns>An <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating the specified <paramref name="email" /> is invalid.</returns>
        public override IdentityError InvalidEmail(string email)
        {
            return Error(base.InvalidEmail(email), ErrorDescriptor.InvalidEmail, email);
        }

        /// <summary>
        /// Returns an <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating the specified <paramref name="role" /> name is invalid.
        /// </summary>
        /// <param name="role">The invalid role.</param>
        /// <returns>An <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating the specific role <paramref name="role" /> name is invalid.</returns>
        public override IdentityError InvalidRoleName(string role)
        {
            return Error(base.InvalidRoleName(role), ErrorDescriptor.InvalidRoleName, role);
        }

        /// <summary>
        /// Returns an <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating an invalid token.
        /// </summary>
        /// <returns>An <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating an invalid token.</returns>
        public override IdentityError InvalidToken()
        {
            return Error(base.InvalidToken(), ErrorDescriptor.InvalidToken);
        }

        /// <summary>
        /// Returns an <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating the specified user <paramref name="userName" /> is invalid.
        /// </summary>
        /// <param name="userName">The user name that is invalid.</param>
        /// <returns>An <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating the specified user <paramref name="userName" /> is invalid.</returns>
        public override IdentityError InvalidUserName(string userName)
        {
            return Error(base.InvalidUserName(userName), ErrorDescriptor.InvalidUserName, userName);
        }

        /// <summary>
        /// Returns an <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating an external login is already associated with an account.
        /// </summary>
        /// <returns>An <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating an external login is already associated with an account.</returns>
        public override IdentityError LoginAlreadyAssociated()
        {
            return Error(base.LoginAlreadyAssociated(), ErrorDescriptor.LoginAlreadyAssociated);
        }

        /// <summary>
        /// Returns an <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a password mismatch.
        /// </summary>
        /// <returns>An <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a password mismatch.</returns>
        public override IdentityError PasswordMismatch()
        {
            return Error(base.PasswordMismatch(), ErrorDescriptor.PasswordMismatch);
        }

        /// <summary>
        /// Returns an <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a password entered does not contain a numeric character, which is required by the password policy.
        /// </summary>
        /// <returns>An <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a password entered does not contain a numeric character.</returns>
        public override IdentityError PasswordRequiresDigit()
        {
            return Error(base.PasswordRequiresDigit(), ErrorDescriptor.PasswordRequiresDigit);
        }

        /// <summary>
        /// Returns an <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a password entered does not contain a lower case letter, which is required by the password policy.
        /// </summary>
        /// <returns>An <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a password entered does not contain a lower case letter.</returns>
        public override IdentityError PasswordRequiresLower()
        {
            return Error(base.PasswordRequiresLower(), ErrorDescriptor.PasswordRequiresLower);
        }

        /// <summary>
        /// Returns an <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a password entered does not contain a non-alphanumeric character, which is required by the password policy.
        /// </summary>
        /// <returns>An <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a password entered does not contain a non-alphanumeric character.</returns>
        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return Error(base.PasswordRequiresNonAlphanumeric(), ErrorDescriptor.PasswordRequiresNonAlphanumeric);
        }

        /// <summary>
        /// Returns an <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a password does not meet the minimum number <paramref name="uniqueChars" /> of unique chars.
        /// </summary>
        /// <param name="uniqueChars">The number of different chars that must be used.</param>
        /// <returns>An <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a password does not meet the minimum number <paramref name="uniqueChars" /> of unique chars.</returns>
        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        {
            return Error(base.PasswordRequiresUniqueChars(uniqueChars), ErrorDescriptor.PasswordRequiresUniqueChars, uniqueChars);
        }

        /// <summary>
        /// Returns an <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a password entered does not contain an upper case letter, which is required by the password policy.
        /// </summary>
        /// <returns>An <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a password entered does not contain an upper case letter.</returns>
        public override IdentityError PasswordRequiresUpper()
        {
            return Error(base.PasswordRequiresUpper(), ErrorDescriptor.PasswordRequiresUpper);
        }

        /// <summary>
        /// Returns an <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a password of the specified <paramref name="length" /> does not meet the minimum length requirements.
        /// </summary>
        /// <param name="length">The length that is not long enough.</param>
        /// <returns>An <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a password of the specified <paramref name="length" /> does not meet the minimum length requirements.</returns>
        public override IdentityError PasswordTooShort(int length)
        {
            return Error(base.PasswordTooShort(length), ErrorDescriptor.PasswordTooShort, length);
        }

        /// <summary>
        /// Returns an <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a recovery code was not redeemed.
        /// </summary>
        /// <returns>An <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a recovery code was not redeemed.</returns>
        public override IdentityError RecoveryCodeRedemptionFailed()
        {
            return Error(base.RecoveryCodeRedemptionFailed(), ErrorDescriptor.RecoveryCodeRedemptionFailed);
        }

        /// <summary>
        /// Returns an <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a user already has a password.
        /// </summary>
        /// <returns>An <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a user already has a password.</returns>
        public override IdentityError UserAlreadyHasPassword()
        {
            return Error(base.UserAlreadyHasPassword(), ErrorDescriptor.UserAlreadyHasPassword);
        }

        /// <summary>
        /// Returns an <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a user is already in the specified <paramref name="role" />.
        /// </summary>
        /// <param name="role">The duplicate role.</param>
        /// <returns>An <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a user is already in the specified <paramref name="role" />.</returns>
        public override IdentityError UserAlreadyInRole(string role)
        {
            return Error(base.UserAlreadyInRole(role), ErrorDescriptor.UserAlreadyInRole, role);
        }

        /// <summary>
        /// Returns an <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating user lockout is not enabled.
        /// </summary>
        /// <returns>An <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating user lockout is not enabled.</returns>
        public override IdentityError UserLockoutNotEnabled()
        {
            return Error(base.UserLockoutNotEnabled(), ErrorDescriptor.UserLockoutNotEnabled);
        }

        /// <summary>
        /// Returns an <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a user is not in the specified <paramref name="role" />.
        /// </summary>
        /// <param name="role">The duplicate role.</param>
        /// <returns>An <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" /> indicating a user is not in the specified <paramref name="role" />.</returns>
        public override IdentityError UserNotInRole(string role)
        {
            return Error(base.UserNotInRole(role), ErrorDescriptor.UserNotInRole, role);
        }
    }
}