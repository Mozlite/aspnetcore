using MS.Extensions.Security.Activities;

namespace MS.Areas.Security.Pages.Account
{
    /// <summary>
    /// 日志模型。
    /// </summary>
    public class LogModel : ModelBase
    {
        private readonly IActivityManager _activityManager;

        public LogModel(IActivityManager activityManager)
        {
            _activityManager = activityManager;
        }

        public ActivityQuery Model { get; private set; }

        public void OnGet(ActivityQuery query)
        {
            query.UserId = UserId;
            Model = _activityManager.Load(query);
        }
    }
}