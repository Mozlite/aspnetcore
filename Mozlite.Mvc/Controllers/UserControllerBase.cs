using Microsoft.AspNetCore.Authorization;
using Mozlite.Extensions.Security;

namespace Mozlite.Mvc.Controllers
{
    /// <summary>
    /// 用户中心控制器。
    /// </summary>
    [Authorize(Roles = IdentitySettings.Register)]
    public abstract class UserControllerBase : ControllerBase
    {

    }
}