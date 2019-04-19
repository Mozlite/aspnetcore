using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite.Extensions.Settings;
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
        private readonly ISettingDictionaryManager _settingDictionaryManager;

        public ExtensionNameDropdownListTagHelper(IMediaDirectory mediaDirectory, ISettingDictionaryManager settingDictionaryManager)
        {
            _mediaDirectory = mediaDirectory;
            _settingDictionaryManager = settingDictionaryManager;
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
                items.Add(new SelectListItem(_settingDictionaryManager.GetOrAddSettings($"extensionname.{extensionName}"), extensionName));
            }

            return items;
        }
    }
}