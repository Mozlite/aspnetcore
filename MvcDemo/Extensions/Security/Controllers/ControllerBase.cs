using Microsoft.AspNetCore.Mvc;

namespace Demo.Extensions.Security.Controllers
{
    /// <summary>
    /// 控制器基类。
    /// </summary>
    [Area(SecuritySettings.ExtensionName)]
    public abstract class ControllerBase : Mozlite.Mvc.ControllerBase
    {

    }
}