using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Mozlite.Data;
using Mozlite.Extensions.Storages.Properties;

namespace Mozlite.Extensions.Storages
{
    /// <summary>
    /// 媒体文件提供者实现类。
    /// </summary>
    public class MediaDirectory : IMediaDirectory
    {
        private readonly IStorageDirectory _directory;
        private readonly IDbContext<MediaFile> _mfdb;
        private readonly IDbContext<StoredFile> _sfdb;

        private const string UserAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
        private readonly string _media;

        /// <summary>
        /// 初始化类<see cref="MediaDirectory"/>。
        /// </summary>
        /// <param name="directory">存储文件夹。</param>
        /// <param name="mfdb">数据库操作接口。</param>
        /// <param name="sfdb">数据库操作接口。</param>
        public MediaDirectory(IStorageDirectory directory, IDbContext<MediaFile> mfdb, IDbContext<StoredFile> sfdb)
        {
            _directory = directory;
            _mfdb = mfdb;
            _sfdb = sfdb;
            //媒体文件夹。
            _media = directory.GetPhysicalPath("media");
        }

        /// <summary>
        /// 上传文件。
        /// </summary>
        /// <param name="file">表单文件。</param>
        /// <param name="extensionName">扩展名称。</param>
        /// <param name="targetId">目标Id。</param>
        /// <param name="uniqueMediaFile">每一个文件和媒体存储文件一一对应。</param>
        /// <returns>返回上传后的结果！</returns>
        public async Task<MediaResult> UploadAsync(IFormFile file, string extensionName, int? targetId = null, bool uniqueMediaFile = true)
        {
            if (file == null || file.Length == 0)
                return new MediaResult(null, Resources.FormFileInvalid);
            var tempFile = _directory.GetTempPath(Guid.NewGuid().ToString());
            using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fs);
            }
            var media = new MediaFile();
            media.ExtensionName = extensionName;
            media.Extension = Path.GetExtension(file.FileName);
            media.Name = file.FileName;
            return await CreateAsync(new FileInfo(tempFile), media, file.ContentType, targetId, uniqueMediaFile);
        }

        /// <summary>
        /// 下载文件。
        /// </summary>
        /// <param name="url">文件URL地址。</param>
        /// <param name="extensionName">扩展名称。</param>
        /// <param name="targetId">目标Id。</param>
        /// <param name="uniqueMediaFile">每一个文件和媒体存储文件一一对应。</param>
        /// <returns>返回上传后的结果！</returns>
        public async Task<MediaResult> DownloadAsync(string url, string extensionName, int? targetId = null, bool uniqueMediaFile = true)
        {
            var uri = new Uri(url);
            using (var client = new HttpClient())
            {
                var tempFile = _directory.GetTempPath(Guid.NewGuid().ToString());
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
                media.ExtensionName = extensionName;
                media.Extension = Path.GetExtension(uri.AbsolutePath);
                media.Name = Path.GetFileName(uri.AbsolutePath);
                return await CreateAsync(new FileInfo(tempFile), media, media.Extension.GetContentType(), targetId, uniqueMediaFile);
            }
        }

        private async Task<MediaResult> CreateAsync(FileInfo tempFile, MediaFile file, string contentType, int? targetId, bool uniqueMediaFile)
        {
            var fileId = tempFile.ComputeHash();
            var storage = await _sfdb.FindAsync(fileId);
            if (storage != null)
            {
                //实体文件已经存在，删除临时目录下的文件
                EnsureStoredFile(storage, tempFile);
                if (uniqueMediaFile)//唯一文件存储
                {
                    var dbFile = await _mfdb.FindAsync(x => x.FileId == fileId);
                    if (dbFile != null)
                        return new MediaResult(dbFile.Url);
                }
            }
            else
            {
                //如果实体文件不存在则创建
                storage = new StoredFile();
                storage.ContentType = contentType;
                storage.FileId = fileId;
                storage.Length = tempFile.Length;
                if (await _sfdb.CreateAsync(storage))
                    EnsureStoredFile(storage, tempFile);
            }
            file.TargetId = targetId;
            file.FileId = fileId;
            if (await _mfdb.CreateAsync(file)) return new MediaResult(file.Url);
            return new MediaResult(null, Resources.StoredFileFailured);
        }

        private void EnsureStoredFile(StoredFile file, FileInfo tempFile)
        {
            //将文件移动到媒体存储路径下。
            var mediaPath = Path.Combine(_media, file.Path);
            if (File.Exists(mediaPath))
            {//如果已经有磁盘文件，删除临时文件
                tempFile.Delete();
            }
            else
            {
                var dir = Path.GetDirectoryName(mediaPath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                tempFile.MoveTo(mediaPath);
            }
        }

        /// <summary>
        /// 通过GUID获取存储文件实例。
        /// </summary>
        /// <param name="id">媒体文件Id。</param>
        /// <returns>返回存储文件实例。</returns>
        public async Task<StoredPhysicalFile> FindAsync(Guid id)
        {
            var file = await _sfdb.AsQueryable().InnerJoin<MediaFile>((sf, mf) => sf.FileId == mf.FileId)
                .Where<MediaFile>(x => x.Id == id)
                .Select<MediaFile>(x => x.Name)
                .Select(x => new { x.FileId, x.ContentType })
                .FirstOrDefaultAsync(reader => new StoredPhysicalFile(reader));
            file.PhysicalPath = Path.Combine(_media, file.PhysicalPath);
            return file;
        }
    }
}