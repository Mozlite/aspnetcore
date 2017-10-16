using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Mozlite.Extensions.Storages
{
    /// <summary>
    /// 存储控制器。
    /// </summary>
    public class StorageController : Controller
    {
        private readonly IMediaFileProvider _mediaFileProvider;
        /// <summary>
        /// 初始化类<see cref="StorageController"/>。
        /// </summary>
        /// <param name="mediaFileProvider">媒体文件提供者接口。</param>
        public StorageController(IMediaFileProvider mediaFileProvider)
        {
            _mediaFileProvider = mediaFileProvider;
        }
        
        /// <summary>
        /// 访问媒体文件。
        /// </summary>
        /// <param name="type">媒体类型。</param>
        /// <param name="name">文件名称。</param>
        /// <returns>返回文件结果。</returns>
        [Route("{type}-medias/{name}")]
        public async Task<IActionResult> Index(string type, string name)
        {
            name = Path.GetFileNameWithoutExtension(name);
            if (!Guid.TryParse(name, out var id))
                return NotFound();
            var file = await _mediaFileProvider.FindAsync(id);
            if (file == null)
                return NotFound();
            return PhysicalFile(file.PhysicalPath, file.ContentType);
        }

        /// <summary>
        /// 访问媒体文件。
        /// </summary>
        /// <param name="type">媒体类型。</param>
        /// <param name="name">文件名称。</param>
        /// <returns>返回文件结果。</returns>
        [Route("{type}-attachments/{name}")]
        public async Task<IActionResult> Attachment(string type, string name)
        {
            name = Path.GetFileNameWithoutExtension(name);
            if (!Guid.TryParse(name, out var id))
                return NotFound();
            var file = await _mediaFileProvider.FindAsync(id);
            if (file == null)
                return NotFound();
            Response.Headers.Add("Content-Disposition", $"attachment;filename={file.FileName}");
            return PhysicalFile(file.PhysicalPath, file.ContentType);
        }
    }
}