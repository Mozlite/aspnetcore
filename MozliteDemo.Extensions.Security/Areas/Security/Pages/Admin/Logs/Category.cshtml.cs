using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security.Events;
using Mozlite.Extensions.Security.Permissions;

namespace MozliteDemo.Extensions.Security.Areas.Security.Pages.Admin.Logs
{
    /// <summary>
    /// 日志分类。
    /// </summary>
    [PermissionAuthorize(Mozlite.Extensions.Permissions.Administrator)]
    public class CategoryModel : ModelBase
    {
        private readonly IEventTypeManager _eventTypeManager;
        public CategoryModel(IEventTypeManager eventTypeManager)
        {
            _eventTypeManager = eventTypeManager;
        }

        public IEnumerable<EventType> Types { get; private set; }

        public void OnGet()
        {
            Types = _eventTypeManager.Fetch();
        }
    }
}