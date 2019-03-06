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
        public new User User => _user ?? (_user = HttpContext.GetUser<User>());

        private Role _role;
        /// <summary>
        /// 当前用户角色。
        /// </summary>
        public Role Role => _role ?? (_role = GetRequiredService<IRoleManager>().FindById(User.RoleId));
    }
}