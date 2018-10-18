using Mozlite.Extensions.Security.Permissions;
using MS.Extensions.Security.Activities;

namespace MS.Areas.Security.Pages.Admin.Logs
{
    /// <summary>
    /// 日志。
    /// </summary>
    [PermissionAuthorize(Security.Permissions.Logs)]
    public class IndexModel : ModelBase
    {
        private readonly IActivityManager _activityManager;

        public IndexModel(IActivityManager activityManager)
        {
            _activityManager = activityManager;
        }

        public UserActivityQuery Model { get; private set; }

        public void OnGet(UserActivityQuery query)
        {
            query.MaxRoleLevel = Role.RoleLevel;
            Model = _activityManager.Load(query);
        }
    }
}