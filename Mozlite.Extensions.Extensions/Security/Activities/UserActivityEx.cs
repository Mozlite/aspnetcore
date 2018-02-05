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

        /// <summary>
        /// 初始化类<see cref="UserActivityEx"/>。
        /// </summary>
        /// <param name="activity">用户活动状态实例。</param>
        public UserActivityEx(UserActivity activity)
        {
            Id = activity.Id;
            UserId = activity.UserId;
            Activity = activity.Activity;
            IPAdress = activity.IPAdress;
            CreatedDate = activity.CreatedDate;
            UserName = activity.UserName;
            NickName = activity.NickName;
        }

        /// <summary>
        /// 初始化类<see cref="UserActivityEx"/>。
        /// </summary>
        public UserActivityEx() { }
    }
}