using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security.Activities;
using Mozlite.Extensions.Settings;
using Mozlite.Extensions.Storages.Excels;
using MS.Extensions.Security;

namespace MS.Areas.Security
{
    /// <summary>
    /// 页面模型基类。
    /// </summary>
    public abstract class ModelBase : Mozlite.Mvc.ModelBase
    {
        private User _user;
        /// <summary>
        /// 获取当前登陆用户。
        /// </summary>
        public User CurrentUser => _user ?? (_user = GetRequiredService<IUserManager>().GetUser());

        private Role _role;
        /// <summary>
        /// 当前用户的角色实例。
        /// </summary>
        public Role CurrentRole => _role ?? (_role = GetRequiredService<IRoleManager>().FindById(CurrentUser.RoleId));

        private SecuritySettings _settings;
        /// <summary>
        /// 安全配置。
        /// </summary>
        public SecuritySettings Settings => _settings ??
                                            (_settings = GetRequiredService<ISettingsManager>()
                                                .GetSettings<SecuritySettings>());

        /// <summary>
        /// 添加操作日志。
        /// </summary>
        /// <param name="message">日志消息。</param>
        /// <param name="args">格式化参数。</param>
        protected void Log(string message, params object[] args)
        {
            Logger.Info(SecuritySettings.EventId, message, args);
        }

        private IExcelManager _excelManager;
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
            if (_excelManager == null) _excelManager = GetRequiredService<IExcelManager>();
            return _excelManager.Export(models, fileName);
        }
    }
}