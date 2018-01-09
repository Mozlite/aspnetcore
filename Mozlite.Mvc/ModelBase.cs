using System;
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

        /// <summary>
        /// 判断验证码。
        /// </summary>
        /// <param name="key">当前唯一键。</param>
        /// <param name="code">验证码。</param>
        /// <returns>返回判断结果。</returns>
        protected virtual bool IsValidateCode(string key, string code)
        {
            if (string.IsNullOrEmpty(code) || !Request.Cookies.TryGetValue(key, out var value))
                return false;
            code = Verifiers.Verifiers.Hashed(code);
            return string.Equals(value, code, StringComparison.OrdinalIgnoreCase);
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

    /// <summary>
    /// 页面模型基类。
    /// </summary>
    /// <typeparam name="TModel">模型实例类型。</typeparam>
    public abstract class ModelBase<TModel> : ModelBase
    {
        /// <summary>
        /// 当前模型实例。
        /// </summary>
        public TModel Model { get; set; }
    }
}