namespace Mozlite.Extensions.Html
{
    /// <summary>
    /// 模板状态。
    /// </summary>
    public enum TemplateStatus
    {
        /// <summary>
        /// 成功。
        /// </summary>
        Succeeded = 0,
        /// <summary>
        /// 失败。
        /// </summary>
        Failured = 1,
        /// <summary>
        /// 配置丢失。
        /// </summary>
        ConfigMissing = 2,
    }
}