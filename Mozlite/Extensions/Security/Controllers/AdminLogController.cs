using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security.Activities;
using Mozlite.Extensions.Security.Models;

namespace Mozlite.Extensions.Security.Controllers
{
    [Authorize]
    public class AdminLogController : ControllerBase
    {
        private readonly IActivityManager _activityManager;

        public AdminLogController(IActivityManager activityManager)
        {
            _activityManager = activityManager;
        }


        public IActionResult Index(ActivityQuery query)
        {
            return View(_activityManager.Load<ActivityQuery, User>(query));
        }
    }
}