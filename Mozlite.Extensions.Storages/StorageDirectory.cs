using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Mozlite.Extensions.Storages.Properties;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Storages
{
    /// <summary>
    /// 存储文件夹提供者实现类。
    /// </summary>
    public class StorageDirectory : IStorageDirectory
    {
        private readonly string _root;
        private readonly string _temp;
        /// <summary>
        /// 初始化类<see cref="StorageDirectory"/>。
        /// </summary>
        /// <param name="configuration">配置接口。</param>
        public StorageDirectory(IConfiguration configuration)
        {
            var path = configuration["StorageDir"]?.Trim() ?? "../storages";
            if (path.StartsWith("~/"))//虚拟目录
                _root = Path.Combine(Directory.GetCurrentDirectory(), path.Substring(2));
            else if (path.Length > 2 && path[1] == ':')//物理目录
                _root = path;
            else
                _root = Path.Combine(Directory.GetCurrentDirectory(), path);
            if (!Directory.Exists(_root)) Directory.CreateDirectory(_root);
            _temp = Path.Combine(_root, "temp");
            if (!Directory.Exists(_temp)) Directory.CreateDirectory(_temp);
        }

        /// <summary>
        /// 获取当前路径的物理路径。
        /// </summary>
        /// <param name="path">当前相对路径。</param>
        /// <returns>返回当前路径的物理路径。</returns>
        public string GetPhysicalPath(string path = null)
        {
            if (path == null)
                return _root;
            path = path.Trim(' ', '~', '/', '\\');
            return Path.Combine(_root, path);
        }

        /// <summary>
        /// 获取临时目录得物理路径。
        /// </summary>
        /// <param name="fileName">文件名称。</param>
        /// <returns>返回当前临时文件物理路径。</returns>
        public string GetTempPath(string fileName = null)
        {
            if (fileName == null)
                return _temp;
            return Path.Combine(_temp, fileName);
        }

        /// <summary>
        /// 获取当前路径文件的操作提供者接口实例。
        /// </summary>
        /// <param name="path">文件相对路径。</param>
        /// <returns>文件的操作提供者接口实例。</returns>
        public IStorageFile GetFile(string path)
        {
            path = GetPhysicalPath(path);
            return new StorageFile(path);
        }

        /// <summary>
        /// 将表单文件实例保存到临时文件夹中。
        /// </summary>
        /// <param name="file">表单文件实例。</param>
        /// <returns>返回文件实例。</returns>
        public virtual async Task<FileInfo> SaveToTempAsync(IFormFile file)
        {
            var tempFile = GetTempPath(Guid.NewGuid().ToString());
            using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fs);
            }
            return new FileInfo(tempFile);
        }

        /// <summary>
        /// 将表单文件实例保存到临时文件夹中。
        /// </summary>
        /// <param name="file">表单文件实例。</param>
        /// <returns>返回文件实例。</returns>
        public virtual async Task<FileInfo> SaveToTempAsync(Stream file)
        {
            var tempFile = GetTempPath(Guid.NewGuid().ToString());
            await file.SaveToAsync(tempFile);
            return new FileInfo(tempFile);
        }

        /// <summary>
        /// 将字符串保存到临时文件夹中。
        /// </summary>
        /// <param name="text">要保存的字符串。</param>
        /// <param name="fileName">文件名。</param>
        /// <returns>返回文件实例。</returns>
        public virtual async Task<FileInfo> SaveToTempAsync(string text, string fileName = null)
        {
            fileName = fileName ?? Guid.NewGuid().ToString();
            fileName = GetTempPath(fileName);
            await StorageHelper.SaveTextAsync(fileName, text);
            return new FileInfo(fileName);
        }

        /// <summary>
        /// 将表单文件实例保存到特定的文件夹中。
        /// </summary>
        /// <param name="file">表单文件实例。</param>
        /// <param name="directoryName">文件夹名称。</param>
        /// <param name="fileName">文件名称，如果为空，则直接使用表单实例的文件名。</param>
        /// <returns>返回文件提供者操作接口实例。</returns>
        public async Task<IStorageFile> SaveAsync(IFormFile file, string directoryName, string fileName = null)
        {
            if (file == null || file.Length == 0)
                throw new Exception(Resources.FormFileInvalid);
            if (fileName != null && fileName.EndsWith(".$"))
                fileName = fileName.Substring(0, fileName.Length - 2) + Path.GetExtension(file.FileName);
            else
                fileName = file.FileName;
            directoryName = GetPhysicalPath(directoryName);
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
            fileName = Path.Combine(directoryName, fileName);
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fs);
            }
            return new StorageFile(fileName);
        }

        /// <summary>
        /// 将字符串保存到特定的文件夹中。
        /// </summary>
        /// <param name="text">要保存的字符串。</param>
        /// <param name="directoryName">文件夹名称。</param>
        /// <param name="fileName">文件名称，如果为空，则直接使用表单实例的文件名。</param>
        /// <returns>返回文件提供者操作接口实例。</returns>
        public virtual async Task<IStorageFile> SaveAsync(string text, string directoryName, string fileName)
        {
            directoryName = GetPhysicalPath(directoryName);
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
            fileName = Path.Combine(directoryName, fileName);
            await StorageHelper.SaveTextAsync(fileName, text);
            return new StorageFile(fileName);
        }

        /// <summary>
        /// 清理空文件夹。
        /// </summary>
        public void ClearEmptyDirectories()
        {
            foreach (var info in new DirectoryInfo(_root).GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                if (info.Name.Equals("temp", StringComparison.OrdinalIgnoreCase))
                {
                    ClearTempFiles(info);
                    continue;
                }
                var directories = info.GetDirectories("*", SearchOption.AllDirectories);
                foreach (var directory in directories)
                {
                    if (!directory.Exists) continue;
                    var files = directory.GetFiles("*", SearchOption.AllDirectories);
                    if (files.Length == 0)
                    {
                        try { directory.Delete(true); }
                        catch { }
                    }
                }
            }
        }

        private void ClearTempFiles(DirectoryInfo info)
        {
            var files = info.GetFiles();
            var expired = DateTime.Now.AddDays(-1);
            foreach (var file in files)
            {
                if (file.LastAccessTime < expired)
                {
                    try { file.Delete(); }
                    catch { }
                }
            }
        }

        private class StorageFile : IStorageFile
        {
            private readonly FileInfo _info;
            public StorageFile(string path)
            {
                _info = new FileInfo(path);
            }

            /// <summary>
            /// 大小。
            /// </summary>
            public long Length => _info.Length;

            /// <summary>
            /// 包含文件夹和文件名全名。
            /// </summary>
            public string FullName => _info.FullName;

            /// <summary>
            /// 文件名称。
            /// </summary>
            public string Name => _info.Name;

            /// <summary>
            /// 扩展名称。
            /// </summary>
            public string Extension => _info.Extension;

            private string _hashed;
            /// <summary>
            /// 文件哈希值，一般为Md5值。
            /// </summary>
            public string Hashed
            {
                get
                {
                    if (_hashed == null && _info.Exists)
                    {
                        _hashed = _info.ComputeHash();
                    }
                    return _hashed;
                }
            }

            /// <summary>
            /// 判断是否存在。
            /// </summary>
            public bool Exists => _info.Exists;

            /// <summary>
            /// 已读取方式打开当前存储文件。
            /// </summary>
            /// <returns>返回文件流。</returns>
            public Stream OpenRead()
            {
                return _info.OpenRead();
            }
        }
    }
}