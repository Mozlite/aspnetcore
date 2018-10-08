using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS.Extensions.Security;
using System.Threading.Tasks;

namespace MS.Areas.Security.Pages.Account
{
    /// <summary>
    /// 更新头像。
    /// </summary>
    public class AvatarModel : ModelBase
    {
        private readonly IUserManager _userManager;

        public AvatarModel(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public void OnGet()
        {
            Input = new InputModel { UserId = UserId };
        }

        public async Task<IActionResult> OnPostAsync(IFormFileCollection files)
        {
            if (Input.AvatarFile == null)
                return RedirectToErrorPage("请选择文件后再上传文件！");
            var result = await _userManager.UploadAvatarAsync(Input.AvatarFile, Input.UserId);
            if (result.Succeeded)
                return RedirectToSuccessPage("你已经成功更新了头像！");
            return ErrorPage(result.Message);
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public IFormFile AvatarFile { get; set; }

            public int UserId { get; set; }
        }
    }
}