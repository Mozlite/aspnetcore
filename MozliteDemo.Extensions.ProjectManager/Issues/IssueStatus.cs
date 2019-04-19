namespace MozliteDemo.Extensions.ProjectManager.Issues
{
    /// <summary>
    /// 状态。
    /// </summary>
    public enum IssueStatus
    {
        /// <summary>
        /// 待解决。
        /// </summary>
        New,
        /// <summary>
        /// 进行中。
        /// </summary>
        Enabled,
        /// <summary>
        /// 已完成。
        /// </summary>
        Completed,
        /// <summary>
        /// 已验收。
        /// </summary>
        Checked,
        /// <summary>
        /// 已拒绝。
        /// </summary>
        Refused,
    }
}