using System.IO;

namespace Mozlite.Extensions.Storages.Caching
{
    /// <summary>
    /// 文件缓存依赖项。
    /// </summary>
    public class FileStorageCacheDependency : StorageCacheDependency
    {
        /// <summary>
        /// 初始化类<see cref="FileStorageCacheDependency"/>。
        /// </summary>
        /// <param name="physicalPath">物理路径。</param>
        public FileStorageCacheDependency(string physicalPath) : base("file", physicalPath)
        {
        }

        /// <summary>
        /// 初始化类<see cref="FileStorageCacheDependency"/>。
        /// </summary>
        /// <param name="file">文件实例。</param>
        public FileStorageCacheDependency(FileInfo file) : this(file.FullName)
        {
        }
    }
}