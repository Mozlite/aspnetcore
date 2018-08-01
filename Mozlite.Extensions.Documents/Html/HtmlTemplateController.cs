using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ControllerBase = Mozlite.Mvc.ControllerBase;

namespace Mozlite.Extensions.Html
{
    public class HtmlTemplateController : ControllerBase
    {
        private readonly ITemplateManager _templateManager;

        public HtmlTemplateController(ITemplateManager templateManager)
        {
            _templateManager = templateManager;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var status = await _templateManager.SetupAsync(file);
            if (status == TemplateStatus.Succeeded)
                return Success(Localizer.GetString(status));
            return Error(Localizer.GetString(status));
        }

        [HttpPost]
        public async Task<IActionResult> Render(Guid id)
        {
            var path = _templateManager.GetTemplatePath(id);
            if (!Directory.Exists(path))
                return Error(Localizer.GetString(TemplateStatus.TemplateNotFound));
            var config = await _templateManager.GetTemplateAsync(id);
            if (config == null)
                return Error(Localizer.GetString(TemplateStatus.ConfigMissing));
            var engine = GetRequiredService<IRazorViewEngine>();
            foreach (var file in Directory.GetFiles("*.cshtml"))
            {
                if(file.StartsWith("_"))
                    continue;
                var page = engine.GetPage(path, file).Page;
                await page.ExecuteAsync();
            }
            return Success("你已经成功生成了代码！");
        }
    }
}