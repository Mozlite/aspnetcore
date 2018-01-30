namespace Mozlite.Extensions
{
    /// <summary>
    /// 支持多站存储。
    /// </summary>
    public interface ISitable
    {
        /// <summary>
        /// 获取当前站Id。
        /// </summary>
        int SiteId { get; set; }
    }
}