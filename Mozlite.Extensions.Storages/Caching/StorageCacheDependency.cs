using System;
using System.IO;
using Mozlite.Extensions.Storages.Properties;

namespace Mozlite.Extensions.Storages.Caching
{
    /// <summary>
    /// 存储缓存依赖项。
    /// </summary>
    public class StorageCacheDependency : IStorageCacheDependency
    {
        /// <summary>
        /// 初始化类<see cref="StorageCacheDependency"/>。
        /// </summary>
        /// <param name="dependency">缓存依赖项的值。</param>
        public StorageCacheDependency(object dependency) : this(null, dependency) { }

        private readonly string _type;
        private readonly object _dependency;
        private string _cache;
        /// <summary>
        /// 初始化类<see cref="StorageCacheDependency"/>。
        /// </summary>
        /// <param name="type">存储类型。</param>
        /// <param name="dependency">缓存依赖项的值。</param>
        protected StorageCacheDependency(string type, object dependency)
        {
            _type = type;
            _dependency = dependency;
        }

        /// <summary>
        /// 是否相等。
        /// </summary>
        /// <param name="dependency">缓存依赖项的值。</param>
        /// <returns>返回判断结果。</returns>
        public bool IsEqual(string dependency)
        {
            return string.Equals(ToString(), dependency, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 是否相等。
        /// </summary>
        /// <param name="dependency">缓存依赖项。</param>
        /// <returns>返回判断结果。</returns>
        public bool IsEqual(IStorageCacheDependency dependency)
        {
            return IsEqual(dependency.ToString());
        }

        private string GetDefaultDependency(object dependency)
        {
            if (dependency == null) return "null";
            if (dependency is string str) return $"string:{str}";
            if (dependency is DateTimeOffset offset) return $"dtos:{offset:yyyy-MM-dd HH:mm:ss}";
            if (dependency is DateTime date) return $"dt:{date:yyyy-MM-dd HH:mm:ss}";
            return $"o:{dependency}";
        }

        /// <summary>
        /// 存储缓存依赖项。
        /// </summary>
        /// <returns>返回存储缓存依赖项的值。</returns>
        public override string ToString()
        {
            if (_cache == null)
            {
                switch (_type)
                {
                    case "file":
                        {
                            var file = new FileInfo(_dependency.ToString());
                            if (file.Exists)
                                _cache = $"file:{file.LastWriteTime:yyyy-MM-dd HH:mm:ss}";
                            else
                                throw new FileNotFoundException(Resources.FileNotFound, file.FullName);
                        }
                        break;
                    case "dir":
                        {
                            var directory = new DirectoryInfo(_dependency.ToString());
                            if (directory.Exists)
                                _cache = $"dir:{directory.LastWriteTime:yyyy-MM-dd HH:mm:ss}";
                            else
                                throw new DirectoryNotFoundException(Resources.DirectoryNotFound);
                        }
                        break;
                    default:
                        _cache = GetDefaultDependency(_dependency);
                        break;
                }
            }
            return _cache;
        }
    }
}