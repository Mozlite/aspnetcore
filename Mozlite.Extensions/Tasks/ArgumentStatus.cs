namespace Mozlite.Extensions.Tasks
{
    /// <summary>
    /// 参数状态。
    /// </summary>
    public enum ArgumentStatus
    {
        /// <summary>
        /// 正常。
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 不在执行了。
        /// </summary>
        Failured = 5,
        /// <summary>
        /// 禁用采集器。
        /// </summary>
        Disabled = 99999,
        /// <summary>
        /// 完成。
        /// </summary>
        Completed = 999999,
    }
}