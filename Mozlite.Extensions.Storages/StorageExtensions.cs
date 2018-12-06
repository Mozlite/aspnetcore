using System;
using System.Drawing;
using System.Drawing.Imaging;
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
            var extension = Path.GetExtension(fileName);
            return extension.IsPicture();
        }

        //按比例缩放 
        private static void GetDrawSize(int srcWidth, int srcHeight, int destWidth, int destHeight, out int width, out int height)
        {
            if (srcHeight > destHeight || srcWidth > destWidth)
            {
                if ((srcWidth * destHeight) > (srcHeight * destWidth))
                {
                    width = destWidth;
                    height = (destWidth * srcHeight) / srcWidth;
                }
                else
                {
                    height = destHeight;
                    width = (srcWidth * destHeight) / srcHeight;
                }
            }
            else
            {
                width = srcWidth;
                height = srcHeight;
            }
        }

        /// <summary>
        /// 缩放图片。
        /// </summary>
        /// <param name="info">图片文件实例，一般在临时文件夹中。</param>
        /// <param name="width">宽度。</param>
        /// <param name="height">高度。</param>
        /// <param name="path">保存路径，未指定将保存在<paramref name="info"/>得文件夹中。</param>
        /// <returns>返回缩略图文件实例。</returns>
        public static FileInfo Resize(this FileInfo info, int width, int height, string path = null)
        {
            if (path == null)
                path = info.DirectoryName;
            else if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, Guid.NewGuid() + ".png");
            var image = Image.FromFile(info.FullName);
            GetDrawSize(image.Width, image.Height, width, height, out var dw, out var dh);
            using (var bitmap = new Bitmap(dw, dh))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.Transparent);
                    //设置画布的描绘质量         
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(image,
                        new Rectangle((width - dw) / 2, (height - dh) / 2, dw, dh), 0, 0,
                        image.Width,
                        image.Height, GraphicsUnit.Pixel);
                }
                bitmap.Save(path, ImageFormat.Png);
            }
            image.Dispose();
            return new FileInfo(path);
        }
    }
}