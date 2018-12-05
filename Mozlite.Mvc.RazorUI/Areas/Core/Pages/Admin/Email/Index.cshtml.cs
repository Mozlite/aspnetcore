using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Messages;
using Mozlite.Extensions.Security.Permissions;

namespace Mozlite.Mvc.RazorUI.Areas.Core.Pages.Admin.Email
{
    [PermissionAuthorize(Permissions.Email)]
    public class IndexModel : AdminModelBase
    {
        private readonly IMessageManager _messageManager;

        public IndexModel(IMessageManager messageManager)
        {
            _messageManager = messageManager;
        }

        [BindProperty(SupportsGet = true)]
        public EmailQuery Model { get; set; }

        public void OnGet()
        {
            Model = _messageManager.Load(Model);
        }
    }
}