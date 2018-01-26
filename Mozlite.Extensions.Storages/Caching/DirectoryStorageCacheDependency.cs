using System.IO;

namespace Mozlite.Extensions.Storages.Caching
{
    /// <summary>
    /// 文件夹缓存依赖项。
    /// </summary>
    public class DirectoryStorageCacheDependency : StorageCacheDependency
    {
        /// <summary>
        /// 初始化类<see cref="DirectoryStorageCacheDependency"/>。
        /// </summary>
        /// <param name="physicalPath">物理路径。</param>
        public DirectoryStorageCacheDependency(string physicalPath) : base("dir", physicalPath)
        {
        }

        /// <summary>
        /// 初始化类<see cref="DirectoryStorageCacheDependency"/>。
        /// </summary>
        /// <param name="dir">文件夹实例。</param>
        public DirectoryStorageCacheDependency(DirectoryInfo dir) : this(dir.FullName)
        {
        }
    }
}