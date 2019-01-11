using Microsoft.AspNetCore.Http;
using Mozlite.Data;
using Mozlite.Extensions.Storages.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

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
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36";
        private readonly string _media;
        private readonly string _thumbs;

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
            //缩略图文件夹
            _thumbs = directory.GetPhysicalPath("media/thumbs");
        }

        /// <summary>
        /// 上传文件。
        /// </summary>
        /// <param name="file">表单文件。</param>
        /// <param name="init">实例化媒体文件属性。</param>
        /// <param name="unique">每一个文件和媒体存储文件一一对应。</param>
        /// <returns>返回上传后的结果！</returns>
        public virtual async Task<MediaResult> UploadAsync(IFormFile file, Action<MediaFile> init, bool unique = true)
        {
            if (file == null || file.Length == 0)
                return new MediaResult(null, Resources.FormFileInvalid);
            var tempFile = await _directory.SaveToTempAsync(file);
            return await SaveAsync(tempFile, file.FileName, init, unique);
        }

        /// <summary>
        /// 上传文件。
        /// </summary>
        /// <param name="file">表单文件。</param>
        /// <param name="extensionName">扩展名称。</param>
        /// <param name="targetId">目标Id。</param>
        /// <param name="uniqueMediaFile">每一个文件和媒体存储文件一一对应。</param>
        /// <returns>返回上传后的结果！</returns>
        public virtual Task<MediaResult> UploadAsync(IFormFile file, string extensionName, int? targetId = null,
            bool uniqueMediaFile = true)
            => UploadAsync(file, x =>
            {
                x.ExtensionName = extensionName;
                x.TargetId = targetId;
            }, uniqueMediaFile);

        /// <summary>
        /// 下载文件。
        /// </summary>
        /// <param name="url">文件URL地址。</param>
        /// <param name="init">实例化媒体文件属性。</param>
        /// <param name="unique">每一个文件和媒体存储文件一一对应。</param>
        /// <returns>返回上传后的结果！</returns>
        public virtual async Task<MediaResult> DownloadAsync(string url, Action<MediaFile> init, bool unique = true)
        {
            var uri = new Uri(url);
            using (var client = new HttpClient())
            {
                FileInfo tempFile;
                client.DefaultRequestHeaders.Referrer = new Uri($"{uri.Scheme}://{uri.DnsSafeHost}{(uri.IsDefaultPort ? null : ":" + uri.Port)}/");
                client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                using (var stream = await client.GetStreamAsync(uri))
                    tempFile = await _directory.SaveToTempAsync(stream);
                return await SaveAsync(tempFile, uri.AbsolutePath, init, unique);
            }
        }

        /// <summary>
        /// 下载文件。
        /// </summary>
        /// <param name="url">文件URL地址。</param>
        /// <param name="extensionName">扩展名称。</param>
        /// <param name="targetId">目标Id。</param>
        /// <param name="unique">每一个文件和媒体存储文件一一对应。</param>
        /// <returns>返回上传后的结果！</returns>
        public Task<MediaResult> DownloadAsync(string url, string extensionName, int? targetId = null, bool unique = true)
            => DownloadAsync(url, x =>
            {
                x.ExtensionName = extensionName;
                x.TargetId = targetId;
            }, unique);

        /// <summary>
        /// 将临时文件存储到系统中。
        /// </summary>
        /// <param name="tempFile">临时文件实例。</param>
        /// <param name="fileName">文件名称，用于解析扩展名和文件名。</param>
        /// <param name="init">实例化媒体文件属性。</param>
        /// <param name="unique">每一个文件和媒体存储文件一一对应。</param>
        /// <returns>返回上传后的结果！</returns>
        public Task<MediaResult> SaveAsync(FileInfo tempFile, string fileName, Action<MediaFile> init, bool unique = true)
        {
            var media = new MediaFile();
            media.Extension = Path.GetExtension(fileName);
            media.Name = Path.GetFileNameWithoutExtension(fileName);
            init(media);
            return CreateAsync(tempFile, media, media.Extension.GetContentType(), unique);
        }

        private async Task<MediaResult> CreateAsync(FileInfo tempFile, MediaFile file, string contentType, bool unique)
        {
            var fileId = tempFile.ComputeHash();
            var storage = await _sfdb.FindAsync(fileId);
            if (storage != null)
            {
                //实体文件已经存在，删除临时目录下的文件
                EnsureStoredFile(storage, tempFile);
                if (unique)//唯一文件存储
                {
                    var dbFile = await _mfdb.FindAsync(x => x.ExtensionName == file.ExtensionName && x.TargetId == file.TargetId && x.FileId == fileId);
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
        public virtual async Task<StoredPhysicalFile> FindAsync(Guid id)
        {
            var file = await _sfdb.AsQueryable().InnerJoin<MediaFile>((sf, mf) => sf.FileId == mf.FileId)
                .Where<MediaFile>(x => x.Id == id)
                .Select<MediaFile>(x => x.Name)
                .Select(x => new { x.FileId, x.ContentType })
                .FirstOrDefaultAsync(reader => new StoredPhysicalFile(reader));
            file.PhysicalPath = Path.Combine(_media, file.PhysicalPath);
            return file;
        }

        /// <summary>
        /// 通过扩展名称和目标Id。
        /// </summary>
        /// <param name="extensionName">扩展名称。</param>
        /// <param name="targetId">目标Id。</param>
        /// <returns>返回媒体文件。</returns>
        public virtual Task<MediaFile> FindAsync(string extensionName, int targetId)
        {
            return _mfdb.FindAsync(x => x.ExtensionName == extensionName && x.TargetId == targetId);
        }

        /// <summary>
        /// 通过扩展名称和目标Id。
        /// </summary>
        /// <param name="extensionName">扩展名称。</param>
        /// <param name="targetId">目标Id。</param>
        /// <returns>返回媒体文件列表。</returns>
        public virtual Task<IEnumerable<MediaFile>> FetchAsync(string extensionName, int targetId)
        {
            return _mfdb.FetchAsync(x => x.ExtensionName == extensionName && x.TargetId == targetId);
        }

        /// <summary>
        /// 加载文件。
        /// </summary>
        /// <param name="query">查询实例。</param>
        /// <returns>返回文件列表。</returns>
        public virtual Task<MediaQuery> LoadAsync(MediaQuery query)
        {
            return _mfdb.LoadAsync(query);
        }

        /// <summary>
        /// 删除文件。
        /// </summary>
        /// <param name="id">文件Id。</param>
        /// <returns>返回删除结果。</returns>
        public virtual Task<bool> DeleteAsync(Guid id)
        {
            return _mfdb.DeleteAsync(id);
        }

        /// <summary>
        /// 删除文件。
        /// </summary>
        /// <param name="extensionName">扩展名称。</param>
        /// <param name="targetId">对象Id。</param>
        /// <returns>返回删除结果。</returns>
        public virtual Task<bool> DeleteAsync(string extensionName, int? targetId = null)
        {
            if (targetId == null)
                return _mfdb.DeleteAsync(x => x.ExtensionName == extensionName);
            return _mfdb.DeleteAsync(x => x.ExtensionName == extensionName && x.TargetId == targetId);
        }

        /// <summary>
        /// 通过GUID获取存储图片文件实例缩略图。
        /// </summary>
        /// <param name="id">媒体文件Id。</param>
        /// <param name="width">宽度。</param>
        /// <param name="height">高度。</param>
        /// <returns>返回存储缩略图实例。</returns>
        public async Task<StoredPhysicalFile> FindThumbAsync(Guid id, int width, int height)
        {
            var file = await _sfdb.AsQueryable().InnerJoin<MediaFile>((sf, mf) => sf.FileId == mf.FileId)
                .Where<MediaFile>(x => x.Id == id)
                .Select<MediaFile>(x => x.Name)
                .Select(x => new { x.FileId, x.ContentType })
                .FirstOrDefaultAsync(reader => new StoredPhysicalFile(reader));
            var thumbFile = new FileInfo(Path.Combine(_thumbs, file.PhysicalPath).Replace(".moz", $".{width}x{height}.moz"));
            if (!thumbFile.Exists)
            {
                var storedFile = new FileInfo(Path.Combine(_media, file.PhysicalPath));
                if (storedFile.Exists)
                {
                    storedFile = storedFile.Resize(width, height, _directory.GetTempPath());
                    if (!Directory.Exists(thumbFile.DirectoryName))
                        Directory.CreateDirectory(thumbFile.DirectoryName);
                    storedFile.MoveTo(thumbFile.FullName);
                }
            }

            file.PhysicalPath = thumbFile.FullName;
            return file;
        }
    }
}