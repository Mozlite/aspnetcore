using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Mozlite.Extensions.Storages.Properties;

namespace Mozlite.Extensions.Storages
{
    /// <summary>
    /// 媒体文件提供者实现类。
    /// </summary>
    public abstract class MediaFileProvider : IMediaFileProvider
    {
        private const string UserAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
        private readonly string _media;
        private readonly string _temp;
        protected MediaFileProvider(IStorageDirectory directoryProvider)
        {
            //媒体文件夹。
            _media = directoryProvider.MapPath("media");
            //临时文件夹。
            _temp = Path.Combine(_media, "temp");
            if (!Directory.Exists(_temp))
                Directory.CreateDirectory(_temp);
        }

        private static readonly string _images = ",.png,.jpg,.jpeg,.gif,.bmp,";
        /// <summary>
        /// 判断是否为图片文件。
        /// </summary>
        /// <param name="extension">文件扩展名。</param>
        /// <returns>返回判断结果。</returns>
        public bool IsImage(string extension)
        {
            if (extension == null)
                return false;
            extension = $",{extension.Trim().ToLower()},";
            return _images.Contains(extension);
        }

        /// <summary>
        /// 上传文件。
        /// </summary>
        /// <param name="file">表单文件。</param>
        /// <param name="extensionName">扩展名称。</param>
        /// <param name="targetId">目标Id。</param>
        /// <returns>返回上传后的结果！</returns>
        public async Task<MediaResult> UploadAsync(IFormFile file, string extensionName, int? targetId)
        {
            if (file == null || file.Length == 0)
                return new MediaResult(null, Resources.FormFileInvalid);
            var tempFile = Path.Combine(_temp, Guid.NewGuid().ToString());
            using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fs);
            }
            var media = new MediaFile();
            media.Extension = Path.GetExtension(file.FileName);
            media.Name = file.FileName;
            return await CreateAsync(new FileInfo(tempFile), media, file.ContentType);
        }

        /// <summary>
        /// 下载文件。
        /// </summary>
        /// <param name="url">文件URL地址。</param>
        /// <param name="extensionName">扩展名称。</param>
        /// <param name="targetId">目标Id。</param>
        /// <returns>返回上传后的结果！</returns>
        public async Task<MediaResult> DownloadAsync(string url, string extensionName, int? targetId)
        {
            var uri = new Uri(url);
            using (var client = new HttpClient())
            {
                var tempFile = Path.Combine(_temp, Guid.NewGuid().ToString());
                client.DefaultRequestHeaders.Referrer = new Uri($"{uri.Scheme}://{uri.DnsSafeHost}{(uri.IsDefaultPort ? null : ":" + uri.Port)}/");
                client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                using (var stream = await client.GetStreamAsync(uri))
                {
                    using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
                    {
                        await stream.CopyToAsync(fs);
                    }
                }
                var media = new MediaFile();
                media.Extension = Path.GetExtension(uri.AbsolutePath);
                media.Name = Path.GetFileName(uri.AbsolutePath);
                return await CreateAsync(new FileInfo(tempFile), media, ContentTypeManager.GetType(media.Extension));
            }
        }

        private async Task<MediaResult> CreateAsync(FileInfo tempFile, MediaFile file, string contentType)
        {
            file.Length = tempFile.Length;
            var storedFile = new StoredFile();
            storedFile.ContentType = contentType;
            storedFile.FileId = tempFile.ComputeHash();
            storedFile.Length = tempFile.Length;
            if (await CreateAsync(storedFile))
            {//将文件移动到媒体存储路径下。
                var mediaPath = Path.Combine(_media, storedFile.Path);
                var dir = Path.GetDirectoryName(mediaPath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.Move(tempFile.FullName, mediaPath);
            }
            file.FileId = storedFile.FileId;
            if (await CreateAsync(file)) return new MediaResult(file.Url);
            return new MediaResult(null, Resources.StoredFileFailured);
        }

        /// <summary>
        /// 添加媒体文件到数据库中。
        /// </summary>
        /// <param name="file">媒体文件实例。</param>
        /// <returns>返回添加结果。</returns>
        protected abstract Task<bool> CreateAsync(MediaFile file);

        /// <summary>
        /// 如果数据库中没有包含当前文件，则添加文件。
        /// </summary>
        /// <param name="file">当前文件实例。</param>
        /// <returns>如果新添加文件返回<c>true</c>，否则返回<c>false</c>。</returns>
        protected abstract Task<bool> CreateAsync(StoredFile file);

        /// <summary>
        /// 通过GUID获取存储文件实例。
        /// </summary>
        /// <param name="id">媒体文件Id。</param>
        /// <returns>返回存储文件实例。</returns>
        public abstract Task<StoredPhysicalFile> FindAsync(Guid id);
    }
}