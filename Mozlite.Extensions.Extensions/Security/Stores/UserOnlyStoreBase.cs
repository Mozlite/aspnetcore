using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Mozlite.Data;
using Mozlite.Extensions.Security.Stores;

namespace Mozlite.Extensions.Extensions.Security.Stores
{
    /// <summary>
    /// 用户存储类型。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TUserClaim">用户声明类型。</typeparam>
    /// <typeparam name="TUserLogin">用户登陆类型。</typeparam>
    /// <typeparam name="TUserToken">用户标识类型。</typeparam>
    public abstract class UserOnlyStoreBase<TUser, TUserClaim, TUserLogin, TUserToken> : Mozlite.Extensions.Security.Stores.UserOnlyStoreBase<TUser, TUserClaim, TUserLogin, TUserToken>
        where TUser : UserBase
        where TUserClaim : UserClaimBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
    {
        /// <summary>
        /// 初始化类<see cref="UserOnlyStoreBase{TUser,TRole, TUserLogin, TUserToken}"/>。
        /// </summary>
        /// <param name="describer">错误描述<see cref="IdentityErrorDescriber"/>实例。</param>
        /// <param name="userContext">用户数据库接口。</param>
        /// <param name="userClaimContext">用户声明数据库接口。</param>
        /// <param name="userLoginContext">用户登陆数据库接口。</param>
        /// <param name="userTokenContext">用户标识数据库接口。</param>
        protected UserOnlyStoreBase(IdentityErrorDescriber describer, IDbContext<TUser> userContext, IDbContext<TUserClaim> userClaimContext, IDbContext<TUserLogin> userLoginContext, IDbContext<TUserToken> userTokenContext)
            : base(describer, userContext, userClaimContext, userLoginContext, userTokenContext)
        {
        }

        /// <summary>
        /// 判断当前用户名称是否存在。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <returns>返回判断结果。</returns>
        public override IdentityResult IsDuplicated(TUser user)
        {
            if (user.UserName != null && UserContext.Any(x => x.SiteId == user.SiteId && x.UserId != user.UserId && x.UserName == user.UserName))
                return IdentityResult.Failed(ErrorDescriber.DuplicateUserName(user.UserName));
            if (user.UserName != null && UserContext.Any(x => x.SiteId == user.SiteId && x.UserId != user.UserId && x.NormalizedUserName == user.NormalizedUserName))
                return IdentityResult.Failed(ErrorDescriber.DuplicateUserName(user.NormalizedUserName));
            return IdentityResult.Success;
        }

        /// <summary>
        /// 判断当前用户名称是否存在。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回判断结果。</returns>
        public override async Task<IdentityResult> IsDuplicatedAsync(TUser user, CancellationToken cancellationToken = default)
        {
            if (user.UserName != null && await UserContext.AnyAsync(x => x.SiteId == user.SiteId && x.UserId != user.UserId && x.UserName == user.UserName, cancellationToken))
                return IdentityResult.Failed(ErrorDescriber.DuplicateUserName(user.UserName));
            if (user.UserName != null && await UserContext.AnyAsync(x => x.SiteId == user.SiteId && x.UserId != user.UserId && x.NormalizedUserName == user.NormalizedUserName, cancellationToken))
                return IdentityResult.Failed(ErrorDescriber.DuplicateUserName(user.NormalizedUserName));
            return IdentityResult.Success;
        }
    }
}