using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security.Permissions;
using Mozlite.Extensions.Storages;
using Mozlite.Extensions.Storages.Excels;
using System;
using System.Collections.Generic;

namespace Mozlite.Mvc.RazorUI
{
    /// <summary>
    /// 模型基类。
    /// </summary>
    [Authorize]
    public abstract class ModelBase : Mvc.ModelBase
    {
        #region storages
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
        #endregion
    }

    /// <summary>
    /// 管理模型基类。
    /// </summary>
    [PermissionAuthorize(Permissions.Administrator)]
    public abstract class AdminModelBase : ModelBase
    {
    }
}