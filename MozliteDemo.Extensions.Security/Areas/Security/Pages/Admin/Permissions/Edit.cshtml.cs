using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security.Permissions;

namespace MozliteDemo.Extensions.Security.Areas.Security.Pages.Admin.Permissions
{
    public class EditModel : ModelBase
    {
        private readonly ICategoryManager _categoryManager;

        public EditModel(ICategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }

        [BindProperty]
        public Category Input { get; set; }

        public IActionResult OnGet(int id)
        {
            Input = _categoryManager.Find(id);
            if (Input == null)
                return NotFound("分类不存在！");
            return Page();
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(Input.Text))
            {
                ModelState.AddModelError("Input.Text", "显示名称不能为空！");
                return Error();
            }

            var result = _categoryManager.Update(Input.Id, new { Input.Disabled, Input.Text });
            if (result)
                EventLogger.LogUser("更新了权限分类：{0}。", Input.Text);
            return Json(result, Input.Text);
        }
    }
}