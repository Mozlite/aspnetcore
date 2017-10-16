using Microsoft.AspNetCore.Authorization;
using Mozlite.Extensions.Security;

namespace Mozlite.Mvc.Controllers
{
    /// <summary>
    /// 后台管理控制器。
    /// </summary>
    [Authorize(Roles = IdentitySettings.Administrator)]
    public abstract class AdminControllerBase : ControllerBase
    {
        private int _pageIndex = -1;
        /// <summary>
        /// 当前页码。
        /// </summary>
        protected override int PageIndex
        {
            get
            {
                if (_pageIndex == -1)
                {
                    string page = Request.Query["page"];
                    if (!int.TryParse(page, out _pageIndex))
                        _pageIndex = 1;
                    if (_pageIndex < 1)
                        _pageIndex = 1;
                }
                return _pageIndex;
            }
        }
    }
}