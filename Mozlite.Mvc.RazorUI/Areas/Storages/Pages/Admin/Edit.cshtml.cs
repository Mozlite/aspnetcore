using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Security.Events;
using Mozlite.Extensions.Storages;

namespace Mozlite.Mvc.RazorUI.Areas.Storages.Pages.Admin
{
    public class EditModel : ModelBase
    {
        private readonly IMediaDirectory _mediaDirectory;

        public EditModel(IMediaDirectory mediaDirectory)
        {
            _mediaDirectory = mediaDirectory;
        }

        public class InputModel
        {
            public Guid Id { get; set; }

            [Required(ErrorMessage = "文件名称不能为空！")]
            public string Name { get; set; }

            public string Extension { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();
            var file = await _mediaDirectory.FindAsync(id);
            if (file == null)
                return NotFound();
            Input = new InputModel { Id = file.Id, Name = file.Name, Extension = file.Extension };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var file = await _mediaDirectory.FindAsync(Input.Id);
                var result = await _mediaDirectory.RenameAsync(Input.Id, Input.Name);
                if (result)
                {
                    await EventLogger.LogCoreAsync("将文件 “{0}{2}” 修改为 “{1}{2}”。", file.Name, Input.Name, Input.Extension);
                    return Success("你已经成功修改了文件！");
                }
            }

            return Error();
        }
    }
}