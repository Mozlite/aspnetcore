using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security;
using Mozlite.Extensions.Storages;
using Mozlite.Extensions.Storages.Excels;
using MS.Extensions.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mozlite.Extensions;
using Mozlite.Extensions.Security.Activities;

namespace MS.Extensions
{
    /// <summary>
    /// 控制器基类。
    /// </summary>
    public abstract class ControllerBase : Mozlite.Mvc.ControllerBase
    {
        #region users
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
        #endregion

        #region json
        /// <summary>
        /// 返回JSON试图结果。
        /// </summary>
        /// <param name="successLoggerMessage">日志信息。</param>
        /// <param name="result">数据结果。</param>
        /// <param name="args">参数。</param>
        /// <returns>返回JSON试图结果。</returns>
        protected IActionResult Json(string successLoggerMessage, DataResult result, params object[] args)
        {
            if (result.Succeed())
                Logger.Info(successLoggerMessage);
            return Json(result, args);
        }

        /// <summary>
        /// 返回JSON试图结果。
        /// </summary>
        /// <param name="successLoggerMessage">日志信息。</param>
        /// <param name="result">数据结果。</param>
        /// <param name="args">参数。</param>
        /// <returns>返回JSON试图结果。</returns>
        protected IActionResult Json(Func<string> successLoggerMessage, DataResult result, params object[] args)
        {
            if (result.Succeed())
                Logger.Info(successLoggerMessage());
            return Json(result, args);
        }

        /// <summary>
        /// 返回JSON试图结果。
        /// </summary>
        /// <param name="successLoggerMessage">日志信息。</param>
        /// <param name="result">数据结果。</param>
        /// <param name="args">参数。</param>
        /// <returns>返回JSON试图结果。</returns>
        protected async Task<IActionResult> Json(Func<Task<string>> successLoggerMessage, DataResult result, params object[] args)
        {
            if (result.Succeed())
                Logger.Info(await successLoggerMessage());
            return Json(result, args);
        }
        #endregion
    }
}