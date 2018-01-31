namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 用户活动状态。
    /// </summary>
    public class UserActivityEx : UserActivity, ISitable
    {
        /// <summary>
        /// 获取当前站Id。
        /// </summary>
        public int SiteId { get; set; }
    }
}