using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Mozlite.Extensions.Storages.Properties;

namespace Mozlite.Extensions.Storages
{
    /// <summary>
    /// 存储文件夹提供者实现类。
    /// </summary>
    public class StorageDirectoryProvider : IStorageDirectoryProvider
    {
        private readonly DirectoryInfo _directory;
        /// <summary>
        /// 初始化类<see cref="StorageDirectoryProvider"/>。
        /// </summary>
        /// <param name="options">存储选项。</param>
        /// <param name="environment">宿主环境接口。</param>
        public StorageDirectoryProvider(IOptions<StorageOptions> options, IHostingEnvironment environment)
        {
            var path = options.Value.StorageDir?.Trim();
            if (path.StartsWith("~/"))
                path = Path.Combine(environment.WebRootPath, path.Substring(2));
            _directory = new DirectoryInfo(path);
        }

        /// <summary>
        /// 获取当前路径的物理路径。
        /// </summary>
        /// <param name="path">当前相对路径。</param>
        /// <returns>返回当前路径的物理路径。</returns>
        public string PhysicalPath(string path)
        {
            path = path?.Trim(' ', '~', '/', '\\');
            return Path.Combine(_directory.FullName, path);
        }

        /// <summary>
        /// 获取当前路径文件的操作提供者接口实例。
        /// </summary>
        /// <param name="path">文件相对路径。</param>
        /// <returns>文件的操作提供者接口实例。</returns>
        public IStorageFileProvider GetFile(string path)
        {
            path = PhysicalPath(path);
            return new StorageFileProvider(path);
        }

        /// <summary>
        /// 将表单文件实例保存到特定的文件夹中。
        /// </summary>
        /// <param name="file">表单文件实例。</param>
        /// <param name="directoryName">文件夹名称。</param>
        /// <param name="fileName">文件名称，如果为空，则直接使用表单实例的文件名。</param>
        /// <returns>返回文件提供者操作接口实例。</returns>
        public async Task<IStorageFileProvider> SaveAsync(IFormFile file, string directoryName, string fileName = null)
        {
            if (file == null || file.Length == 0)
                throw new Exception(Resources.FormFileInvalid);
            if (fileName != null && fileName.EndsWith(".$"))
                fileName = fileName.Substring(0, fileName.Length - 2) + Path.GetExtension(file.FileName);
            else
                fileName = file.FileName;
            directoryName = PhysicalPath(directoryName);
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
            fileName = Path.Combine(directoryName, fileName);
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fs);
            }
            return new StorageFileProvider(fileName);
        }

        private class StorageFileProvider : IStorageFileProvider
        {
            private readonly FileInfo _info;
            public StorageFileProvider(string path)
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
                        _hashed = StorageHelper.Hashed(_info);
                    }
                    return _hashed;
                }
            }
        }
    }
}