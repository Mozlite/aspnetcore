using Demo.Extensions.Security.Activities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Extensions.Security.Controllers
{
    [Authorize]
    public class AdminLogController : ControllerBase
    {
        private readonly IActivityManager _activityManager;

        public AdminLogController(IActivityManager activityManager)
        {
            _activityManager = activityManager;
        }


        public IActionResult Index(UserActivityQuery query)
        {
            return View(_activityManager.Load(query));
        }
    }
}