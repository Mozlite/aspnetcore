using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Messages.Notifications;
using Mozlite.Extensions.Settings;
using System.Collections.Generic;

namespace Mozlite.Mvc.RazorUI.Areas.Core.Pages.Admin.Notifications
{
    public class IndexModel : AdminModelBase
    {
        private readonly INotificationTypeManager _typeManager;
        private readonly ISettingsManager _settingsManager;

        public IndexModel(INotificationTypeManager typeManager, ISettingsManager settingsManager)
        {
            _typeManager = typeManager;
            _settingsManager = settingsManager;
        }

        /// <summary>
        /// 通知类型列表。
        /// </summary>
        public IEnumerable<NotificationType> Types { get; private set; }

        /// <summary>
        /// 配置。
        /// </summary>
        public NotificationSettings Settings { get; private set; }

        public void OnGet()
        {
            Types = _typeManager.Fetch();
            Settings = _settingsManager.GetSettings<NotificationSettings>();
        }

        public IActionResult OnPostSettings(int size)
        {
            Settings = _settingsManager.GetSettings<NotificationSettings>();
            Settings.MaxSize = size;
            if (_settingsManager.SaveSettings(Settings))
            {
                Log("修改了每个用户最大通知数量为：{0}。", size);
                return Success("你已经成功更新了记录数！");
            }

            return Error("更新记录数失败，请重试！");
        }

        public IActionResult OnPostDelete(int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                return Error("请先选择通知类型后在进行删除操作！");
            }

            var result = _typeManager.Delete(ids);
            return LogResult(result, "通知类型", ids);
        }
    }
}