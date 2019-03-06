using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite.Extensions.Security.Events;
using System.Collections.Generic;

namespace Mozlite.Mvc.TagHelpers.Common
{
    /// <summary>
    /// 事件类型下拉列表框。
    /// </summary>
    [HtmlTargetElement("moz:event-type-dropdownlist")]
    public class EventTypeDropdownListTagHelper : DropdownListTagHelper
    {
        private readonly IEventTypeManager _eventTypeManager;
        /// <summary>
        /// 初始化类<see cref="EventTypeDropdownListTagHelper"/>。
        /// </summary>
        /// <param name="eventTypeManager">事件类型管理接口。</param>
        public EventTypeDropdownListTagHelper(IEventTypeManager eventTypeManager)
        {
            _eventTypeManager = eventTypeManager;
        }

        /// <summary>
        /// 初始化选项列表。
        /// </summary>
        /// <returns>返回选项列表。</returns>
        protected override IEnumerable<SelectListItem> Init()
        {
            var eventTypes = _eventTypeManager.Fetch();
            foreach (var eventType in eventTypes)
            {
                yield return new SelectListItem(eventType.Name, eventType.Id.ToString());
            }
        }
    }
}