using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = Mozlite.Mvc.ControllerBase;

namespace Mozlite.Extensions.Documents.Html
{
    /// <summary>
    /// HTML模板控制器。
    /// </summary>
    public class HtmlTemplateController : ControllerBase
    {
        private readonly ITemplateManager _templateManager;
        /// <summary>
        /// 初始化类<see cref="HtmlTemplateController"/>。
        /// </summary>
        /// <param name="templateManager">模板管理接口。</param>
        public HtmlTemplateController(ITemplateManager templateManager)
        {
            _templateManager = templateManager;
        }

        /// <summary>
        /// 上传模板文件，使用zip压缩文件。
        /// </summary>
        /// <param name="file">文件实例。</param>
        /// <returns>返回试图结果。</returns>
        [HttpPost]
        [Route("html-template/upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var status = await _templateManager.SetupAsync(file);
            if (status == TemplateStatus.Succeeded)
                return Success(Localizer.GetString(status));
            return Error(Localizer.GetString(status));
        }

        /// <summary>
        /// 生成模板文件。
        /// </summary>
        /// <param name="id">配置Id。</param>
        /// <returns>返回试图结果。</returns>
        [Route("html-template/general")]
        public async Task<IActionResult> General(Guid id)
        {
            var path = _templateManager.GetTemplatePath(id);
            if (!Directory.Exists(path))
                return Error(Localizer.GetString(TemplateStatus.TemplateNotFound));
            var config = await _templateManager.GetTemplateAsync(id);
            if (config == null)
                return Error(Localizer.GetString(TemplateStatus.ConfigMissing));
            foreach (var file in Directory.GetFiles(path, "*.cshtml", SearchOption.TopDirectoryOnly))
            {
                if(file.StartsWith("_"))
                    continue;
                await _templateManager.SaveGeneratorAsync(file, config);
            }
            return Success("你已经成功生成了代码！");
        }
    }
}