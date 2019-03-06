namespace Mozlite.Extensions.Security.Events
{
    /// <summary>
    /// 事件管理接口。
    /// </summary>
    public interface IEventManager : IObjectManager<EventMessage>, ISingletonService
    {
    }
}