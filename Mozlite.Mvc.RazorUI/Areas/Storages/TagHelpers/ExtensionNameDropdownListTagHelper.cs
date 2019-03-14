using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite.Extensions.Storages;
using Mozlite.Mvc.TagHelpers;

namespace Mozlite.Mvc.RazorUI.Areas.Storages.TagHelpers
{
    /// <summary>
    /// 扩展名称。
    /// </summary>
    [HtmlTargetElement("moz:extension-name-dropdownlist")]
    public class ExtensionNameDropdownListTagHelper : DropdownListTagHelper
    {
        private readonly IMediaDirectory _mediaDirectory;

        public ExtensionNameDropdownListTagHelper(IMediaDirectory mediaDirectory)
        {
            _mediaDirectory = mediaDirectory;
        }

        /// <summary>
        /// 初始化选项列表。
        /// </summary>
        /// <returns>返回选项列表。</returns>
        protected override async Task<IEnumerable<SelectListItem>> InitAsync()
        {
            var extensionNames = await _mediaDirectory.LoadExtensionNamesAsync();
            var items = new List<SelectListItem>();
            foreach (var extensionName in extensionNames)
            {
                items.Add(new SelectListItem(extensionName, extensionName));
            }

            return items;
        }
    }
}