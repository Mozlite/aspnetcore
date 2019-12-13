using Microsoft.AspNetCore.Authorization;
using Mozlite.Extensions.Security;
using MozliteDemo.Extensions.Security;

namespace MozliteDemo.Extensions
{
    /// <summary>
    /// 模型基类。
    /// </summary>
    [Authorize]
    public abstract class ModelBase : Mozlite.Mvc.ModelBase
    {
        private User _user;
        /// <summary>
        /// 当前用户。
        /// </summary>
        public new User User => _user ??= HttpContext.GetUser<User>();

        private Role _role;
        /// <summary>
        /// 当前用户角色。
        /// </summary>
        public Role Role => _role ??= GetRequiredService<IRoleManager>().FindById(User.RoleId);

        private IUserManager _userManager;
        /// <summary>
        /// 获取缓存用户实例。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回缓存用户实例对象。</returns>
        public CachedUser GetUser(int userId)
        {
            if (_userManager == null)
                _userManager = GetRequiredService<IUserManager>();
            return _userManager.GetUser(userId);
        }
    }
}