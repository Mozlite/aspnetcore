namespace Mozlite
{
    /// <summary>
    /// 服务接口访问器，主要用于高级服务调用低级服务得接口注入。
    /// </summary>
    /// <typeparam name="TService">当前服务接口。</typeparam>
    public interface IServiceAccessor<out TService>
    {
        /// <summary>
        /// 接口实例对象。
        /// </summary>
        TService Service { get; }
    }
}