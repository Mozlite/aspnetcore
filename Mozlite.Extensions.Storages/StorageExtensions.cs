using System;
using System.IO;
using System.Security.Cryptography;

namespace Mozlite.Extensions.Storages
{
    /// <summary>
    /// 扩展类。
    /// </summary>
    public static class StorageExtensions
    {
        /// <summary>
        /// 计算文件的哈希值。
        /// </summary>
        /// <param name="info">文件信息实例。</param>
        /// <returns>返回文件的哈希值。</returns>
        public static string ComputeHash(this FileInfo info)
        {
            using (var fs = new FileStream(info.FullName, FileMode.Open, FileAccess.Read))
            {
                var md5 = MD5.Create();
                return md5.ComputeHash(fs).ToHexString();
            }
        }

        /// <summary>
        /// 判断是否为本地媒体文件路径。
        /// </summary>
        /// <param name="url">当前媒体路径。</param>
        /// <returns>返回判断结果。</returns>
        public static bool IsLocalMediaUrl(this string url)
        {
            if (!url.IsLocalUrl())
                return false;
            var guid = Path.GetFileNameWithoutExtension(url);
            return Guid.TryParse(guid, out var _);
        }

        /// <summary>
        /// 判断是否为本地地址。
        /// </summary>
        /// <param name="url">URL地址。</param>
        /// <returns>返回判断结果。</returns>
        public static bool IsLocalUrl(this string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            if (url[0] == 46)//../
                return true;
            if (url[0] == 47 && (url.Length == 1 || url[1] != 47 && url[1] != 92))//
                return true;
            if (url.Length > 1 && url[0] == 126)//~/
                return url[1] == 47;
            return false;
        }

        internal static string MakedPath(this string md5)
        {
            return $"{md5[1]}\\{md5[3]}\\{md5[12]}\\{md5[16]}\\{md5[20]}\\{md5}.moz";
        }

        private static readonly string _images = ",.png,.jpg,.jpeg,.gif,.bmp,";
        /// <summary>
        /// 判断是否为图片文件。
        /// </summary>
        /// <param name="extension">文件扩展名。</param>
        /// <returns>返回判断结果。</returns>
        public static bool IsPicture(this string extension)
        {
            if (extension == null)
                return false;
            extension = $",{extension.Trim().ToLower()},";
            return _images.Contains(extension);
        }

        /// <summary>
        /// 判断是否为图片文件。
        /// </summary>
        /// <param name="fileName">文件名。</param>
        /// <returns>返回判断结果。</returns>
        public static bool IsPictureFile(this string fileName)
        {
            if (fileName == null)
                return false;
            var extension = Path.GetExtension(fileName);
            if (extension == null)
                return false;
            extension = $",{extension.Trim().ToLower()},";
            return _images.Contains(extension);
        }

        /// <summary>
        /// 转换为缓存依赖项字符串。
        /// </summary>
        /// <param name="dependency">缓存依赖项对象。</param>
        /// <returns>返回缓存依赖项字符串。</returns>
        public static string ToCacheDependency(this object dependency)
        {
            if (dependency == null) return "null";
            if (dependency is string str) return $"string:{str}";
            if (dependency is DateTimeOffset offset) return $"dtos:{offset:yyyy-MM-dd HH:mm:ss}";
            if (dependency is DateTime date) return $"dt:{date:yyyy-MM-dd HH:mm:ss}";
            return $"o:{dependency}";
        }
    }
}