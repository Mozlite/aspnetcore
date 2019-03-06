using Mozlite.Extensions.Categories;

namespace Mozlite.Extensions.Security.Events
{
    /// <summary>
    /// 事件类型管理接口。
    /// </summary>
    public interface IEventTypeManager : ICachableCategoryManager<EventType>, ISingletonService
    {

    }
}