using Microsoft.AspNetCore.Authorization;
using Mozlite.Extensions;
using Mozlite.Extensions.Security.Permissions;

namespace Mozlite.Mvc.RazorUI
{
    /// <summary>
    /// 模型基类。
    /// </summary>
    [Authorize]
    public abstract class ModelBase : Mvc.ModelBase
    {
    }

    /// <summary>
    /// 管理模型基类。
    /// </summary>
    [PermissionAuthorize(Permissions.Administrator)]
    public abstract class AdminModelBase : ModelBase
    {
    }
}