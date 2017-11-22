using Microsoft.AspNetCore.Mvc.RazorPages;
using Mozlite.Extensions;

namespace Mozlite.Mvc
{
    /// <summary>
    /// 页面模型基类。
    /// </summary>
    public abstract class ModelBase : PageModel
    {
        private int _pageIndex = -1;
        /// <summary>
        /// 当前页码。
        /// </summary>
        public virtual int PageIndex
        {
            get
            {
                if (_pageIndex == -1)
                {
                    string page;
                    if (RouteData.Values.TryGetValue("page", out var pobject))
                        page = pobject.ToString();
                    else
                        page = Request.Query["page"];
                    if (!int.TryParse(page, out _pageIndex))
                        _pageIndex = 1;
                    if (_pageIndex < 1)
                        _pageIndex = 1;
                }
                return _pageIndex;
            }
        }

        #region users
        private int _userId = -1;
        /// <summary>
        /// 当前用户ID。
        /// </summary>
        protected int UserId
        {
            get
            {
                if (_userId == -1)
                {
                    _userId = User.GetUserId();
                }
                return _userId;
            }
        }

        private string _userName;

        /// <summary>
        /// 当前用户名称。
        /// </summary>
        protected string UserName => _userName ?? (_userName = User.GetUserName());
        #endregion
    }
}