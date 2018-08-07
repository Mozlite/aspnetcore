namespace Mozlite.Extensions.Documents.Html
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
        /// <summary>
        /// 未找到配置等。
        /// </summary>
        FileNotFound = 3,
        /// <summary>
        /// 模板不存在。
        /// </summary>
        TemplateNotFound = 3,
    }
}