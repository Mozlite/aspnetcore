using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security.Events;
using Mozlite.Extensions.Settings;
using System.Linq;

namespace Mozlite.Mvc.RazorUI.Areas.Core.Pages.Admin.Settings
{
    public class IndexModel : AdminModelBase
    {
        private readonly ISettingDictionaryManager _settingManager;
        public IndexModel(ISettingDictionaryManager settingManager)
        {
            _settingManager = settingManager;
        }

        public SettingDictionary Current { get; private set; }

        public void OnGet(int id = 0)
        {
            Current = _settingManager.Find(id);
        }

        public IActionResult OnPostDelete(int[] ids, int pid)
        {
            if (ids == null || ids.Length == 0)
                return Error("请选择实例后再进行删除操作！");
            var settings = _settingManager.Find(pid).Children.Where(x => ids.Contains(x.Id)).ToList();
            foreach (var setting in settings)
            {
                if (setting.Count > 0)
                    return Error($"{setting.Value} 下面的字典实例不为空，需要先清空子项，才能进行删除操作！");
            }

            var result = _settingManager.Delete(ids);
            if (result)
            {
                EventLogger.LogCore("删除了字典实例：{0}", string.Join(",", settings.Select(x => x.Value)));
            }

            return Json(result, "字典实例");
        }
    }
}