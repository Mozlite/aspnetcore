using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Mozlite.Extensions.Storages.Controllers
{
    /// <summary>
    /// 存储控制器。
    /// </summary>
    public class StorageController : Controller
    {
        private readonly IMediaDirectory _mediaFileProvider;
        /// <summary>
        /// 初始化类<see cref="StorageController"/>。
        /// </summary>
        /// <param name="mediaFileProvider">媒体文件提供者接口。</param>
        public StorageController(IMediaDirectory mediaFileProvider)
        {
            _mediaFileProvider = mediaFileProvider;
        }

        /// <summary>
        /// 访问媒体文件。
        /// </summary>
        /// <param name="name">文件名称。</param>
        /// <returns>返回文件结果。</returns>
        [Route("s-medias/{name}")]
        public async Task<IActionResult> Index(string name)
        {
            name = Path.GetFileNameWithoutExtension(name);
            if (!Guid.TryParse(name, out var id))
                return NotFound();
            var file = await _mediaFileProvider.FindAsync(id);
            if (file == null || !System.IO.File.Exists(file.PhysicalPath))
                return NotFound();
            return PhysicalFile(file.PhysicalPath, file.ContentType);
        }

        /// <summary>
        /// 访问媒体文件。
        /// </summary>
        /// <param name="name">文件名称。</param>
        /// <returns>返回文件结果。</returns>
        [Route("s-download/{name}")]
        public async Task<IActionResult> Attachment(string name)
        {
            name = Path.GetFileNameWithoutExtension(name);
            if (!Guid.TryParse(name, out var id))
                return NotFound();
            var file = await _mediaFileProvider.FindAsync(id);
            if (file == null || !System.IO.File.Exists(file.PhysicalPath))
                return NotFound();
            Response.Headers.Add("Content-Disposition", $"attachment;filename={file.FileName}");
            return PhysicalFile(file.PhysicalPath, file.ContentType);
        }
    }
}