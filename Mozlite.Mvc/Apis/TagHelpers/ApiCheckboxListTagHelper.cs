using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite.Mvc.TagHelpers.Bootstrap;
using System.Collections.Generic;

namespace Mozlite.Mvc.Apis.TagHelpers
{
    /// <summary>
    /// API列表。
    /// </summary>
    [HtmlTargetElement("moz:apis-checkboxlist")]
    public class ApiCheckboxListTagHelper : CheckboxListTagHelper
    {
        private readonly IApiManager _apiManager;

        public ApiCheckboxListTagHelper(IApiManager apiManager)
        {
            _apiManager = apiManager;
        }

        /// <summary>
        /// 分类。
        /// </summary>
        [HtmlAttributeName("type")]
        public int CategoryId { get; set; }

        /// <summary>
        /// 附加复选项目列表，文本/值。
        /// </summary>
        /// <param name="items">复选框项目列表实例。</param>
        protected override void Init(IDictionary<string, string> items)
        {
            var apis = _apiManager.LoadApis(CategoryId);
            foreach (var api in apis)
            {
                items[api.Name] = api.Id.ToString();
            }
        }
    }
}