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
        /// <summary>
        /// 事件ID。
        /// </summary>
        protected override int EventId => RazorUISettings.EventId;
    }

    /// <summary>
    /// 管理模型基类。
    /// </summary>
    [PermissionAuthorize(Permissions.Administrator)]
    public abstract class AdminModelBase : ModelBase
    {
    }
}