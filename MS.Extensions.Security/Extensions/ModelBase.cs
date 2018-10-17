using Mozlite.Extensions.Security;
using MS.Extensions.Security;

namespace MS.Extensions
{
    /// <summary>
    /// 模型基类。
    /// </summary>
    public abstract class ModelBase : Mozlite.Mvc.ModelBase
    {
        #region users
        private User _user;
        /// <summary>
        /// 当前用户。
        /// </summary>
        public new User User => _user ?? (_user = HttpContext.GetUser<User>());

        private Role _role;
        /// <summary>
        /// 当前用户用户组。
        /// </summary>
        public Role Role => _role ?? (_role = GetRequiredService<IRoleManager>().FindById(User.RoleId));
        #endregion
    }
}