using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite.Mvc.TagHelpers;
using System.Collections.Generic;

namespace Mozlite.Mvc.Apis.TagHelpers
{
    /// <summary>
    /// 应用下拉列表框。
    /// </summary>
    [HtmlTargetElement("moz:app-dropdownlist")]
    public class AppDropdownlistTagHelper : DropdownListTagHelper
    {
        private readonly IApiManager _apiManager;

        public AppDropdownlistTagHelper(IApiManager apiManager)
        {
            _apiManager = apiManager;
        }

        /// <summary>
        /// 初始化选项列表。
        /// </summary>
        /// <returns>返回选项列表。</returns>
        protected override IEnumerable<SelectListItem> Init()
        {
            foreach (var application in _apiManager.Load())
            {
                yield return new SelectListItem(application.Name, application.Id.ToString());
            }
        }
    }
}