using Mozlite.Extensions.Security;
using MozliteDemo.Extensions.Security;

namespace MozliteDemo.Extensions
{
    /// <summary>
    /// 控制器基类。
    /// </summary>
    public abstract class ControllerBase : Mozlite.Mvc.ControllerBase
    {
        private User _user;
        /// <summary>
        /// 当前用户。
        /// </summary>
        protected new User User => _user ?? (_user = HttpContext.GetUser<User>());

        private Role _role;
        /// <summary>
        /// 当前用户角色。
        /// </summary>
        protected Role Role => _role ?? (_role = GetRequiredService<IRoleManager>().FindById(User.RoleId));

        private IUserManager _userManager;
        /// <summary>
        /// 获取缓存用户实例。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <returns>返回缓存用户实例对象。</returns>
        protected CachedUser GetUser(int userId)
        {
            if (_userManager == null)
                _userManager = GetRequiredService<IUserManager>();
            return _userManager.GetUser(userId);
        }
    }
}