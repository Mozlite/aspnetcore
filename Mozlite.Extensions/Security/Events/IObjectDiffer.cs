namespace Mozlite.Extensions.Security.Events
{
    /// <summary>
    /// 对象变更对比接口。
    /// </summary>
    public interface IObjectDiffer : IService
    {
        /// <summary>
        /// 实例化一个对象，一般为原有对象实例。
        /// </summary>
        /// <param name="oldInstance">原有对象实例。</param>
        void Init(object oldInstance);

        /// <summary>
        /// 实例化一个对象，一般为原有对象实例。
        /// </summary>
        /// <param name="oldInstance">原有对象实例。</param>
        /// <returns>返回当前实例。</returns>
        T GetAndInit<T>(T oldInstance);

        /// <summary>
        /// 对象新的对象，判断是否已经变更。
        /// </summary>
        /// <param name="newInstance">新对象实例。</param>
        /// <returns>返回对比结果。</returns>
        bool IsDifference(object newInstance);
    }
}