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

        #region json
        /// <summary>
        /// 返回上传/下载结果。
        /// </summary>
        /// <returns>返回JSON对象。</returns>
        /// <param name="result">上传/下载结果。</param>
        protected IActionResult Json(MediaResult result)
        {
            if (result.Succeeded)
                return Success(new { result.Url });
            return Error(result.Message);
        }

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