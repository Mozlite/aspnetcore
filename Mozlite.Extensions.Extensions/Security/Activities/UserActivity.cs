namespace Mozlite.Extensions.Extensions.Security.Activities
{
    /// <summary>
    /// 用户活动状态。
    /// </summary>
    public abstract class UserActivity : Mozlite.Extensions.Security.Activities.UserActivity, ISitable
    {
        /// <summary>
        /// 获取当前站Id。
        /// </summary>
        public int SiteId { get; set; }
    }
}