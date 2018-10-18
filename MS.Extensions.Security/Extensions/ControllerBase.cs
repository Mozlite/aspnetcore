using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security;
using Mozlite.Extensions.Storages.Excels;
using MS.Extensions.Security;
using System;
using System.Collections.Generic;

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
        /// 当前用户用户组。
        /// </summary>
        protected Role Role => _role ?? (_role = GetRequiredService<IRoleManager>().FindById(User.RoleId));
        #endregion

        #region excel
        /// <summary>
        /// 导出Excel。
        /// </summary>
        /// <typeparam name="TModel">模型类型。</typeparam>
        /// <param name="models">模型实例列表。</param>
        /// <param name="fileName">导出文件名称。</param>
        /// <returns>返回试图结果。</returns>
        protected IActionResult Excel<TModel>(IEnumerable<TModel> models, string fileName = null)
            where TModel : class, new()
        {
            if (fileName == null)
                fileName = Guid.NewGuid().ToString("N");
            return GetRequiredService<IExcelManager>().Export(models, fileName);
        }
        #endregion
    }
}