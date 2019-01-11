using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Mozlite.Extensions.Storages.Apis
{
    /// <summary>
    /// 存储控制器。
    /// </summary>
    public class StorageController : Controller
    {
        private readonly IMediaDirectory _mediaFileProvider;
        private readonly IStorageDirectory _storageDirectory;

        /// <summary>
        /// 初始化类<see cref="StorageController"/>。
        /// </summary>
        /// <param name="mediaFileProvider">媒体文件提供者接口。</param>
        /// <param name="storageDirectory">存储文件夹接口。</param>
        public StorageController(IMediaDirectory mediaFileProvider, IStorageDirectory storageDirectory)
        {
            _mediaFileProvider = mediaFileProvider;
            _storageDirectory = storageDirectory;
        }

        /// <summary>
        /// 访问存储文件。
        /// </summary>
        /// <param name="dir">文件夹名称。</param>
        /// <param name="name">文件名称。</param>
        /// <returns>返回文件结果。</returns>
        [Route("s-files/{dir:alpha}/{name}")]
        public IActionResult Index(string dir, string name)
        {
            name = Path.Combine(dir, name);
            var file = _storageDirectory.GetFile(name);
            if (file == null || !file.Exists)
                return NotFound();
            return PhysicalFile(file.FullName, file.Extension.GetContentType());
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
        /// 访问缩略图片文件。
        /// </summary>
        /// <param name="width">宽度。</param>
        /// <param name="height">高度。</param>
        /// <param name="name">文件名称。</param>
        /// <returns>返回文件结果。</returns>
        [Route("s-medias/{width:int}x{height:int}/{name}")]
        public async Task<IActionResult> Index(int width, int height, string name)
        {
            name = Path.GetFileNameWithoutExtension(name);
            if (!Guid.TryParse(name, out var id))
                return NotFound();
            var file = await _mediaFileProvider.FindThumbAsync(id, width, height);
            if (file == null || !System.IO.File.Exists(file.PhysicalPath))
                return NotFound();
            return PhysicalFile(file.PhysicalPath, "image/png");
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