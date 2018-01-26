using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Mozlite.Utils;

namespace Mozlite.Extensions.Storages
{
    /// <summary>
    /// 存储辅助类型。
    /// </summary>
    public static class StorageHelper
    {
        /// <summary>
        /// 下载文件并保存到目录中，如果文件已经存在则不下载。
        /// </summary>
        /// <param name="url">URL地址。</param>
        /// <param name="dir">保存的文件夹物理路径。</param>
        /// <param name="fileNameWithoutExtension">不包含扩展名的文件名。</param>
        /// <returns>返回文件名称。</returns>
        public static async Task<string> DownloadAsync(string url, string dir, string fileNameWithoutExtension)
        {
            var uri = new Uri(url);
            var name = fileNameWithoutExtension == null ?
                Path.GetFileName(uri.AbsolutePath) :
                fileNameWithoutExtension + Path.GetExtension(uri.AbsolutePath);
            var path = Path.Combine(dir, name);
            if (File.Exists(path))
                return name;

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return await HttpHelper.ExecuteAsync(async client =>
            {
                using (var stream = await client.GetStreamAsync(uri))
                {
                    await stream.SaveToAsync(path);
                }
                return name;
            });
        }

        /// <summary>
        /// 下载文件并保存到目录中，如果文件已经存在则不下载。
        /// </summary>
        /// <param name="url">URL地址。</param>
        /// <param name="path">保存文件（临时文件）物理路径。</param>
        /// <returns>返回文件扩展名称。</returns>
        public static async Task<string> DownloadAsync(string url, string path)
        {
            try
            {
                var uri = new Uri(url);
                return await HttpHelper.ExecuteAsync(async client =>
                {
                    using (var stream = await client.GetStreamAsync(uri))
                    {
                        await stream.SaveToAsync(path);
                    }
                    return Path.GetExtension(uri.AbsolutePath);
                });
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 读取所有文件内容。
        /// </summary>
        /// <param name="path">文件的物理路径。</param>
        /// <param name="share">文件共享选项。</param>
        /// <returns>返回文件内容字符串。</returns>
        public static string ReadText(string path, FileShare share = FileShare.None)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, share))
            {
                using (var reader = new StreamReader(fs, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// 读取所有文件内容。
        /// </summary>
        /// <param name="path">文件的物理路径。</param>
        /// <param name="share">文件共享选项。</param>
        /// <returns>返回文件内容字符串。</returns>
        public static async Task<string> ReadTextAsync(string path, FileShare share = FileShare.None)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, share))
            {
                using (var reader = new StreamReader(fs, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        /// <summary>
        /// 保存文件内容。
        /// </summary>
        /// <param name="path">文件的物理路径。</param>
        /// <param name="text"></param>
        /// <param name="share">文件共享选项。</param>
        /// <returns>返回写入任务实例对象。</returns>
        public static void SaveText(string path, string text, FileShare share = FileShare.None)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, share))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.Write(text);
                }
            }
        }

        /// <summary>
        /// 保存文件内容。
        /// </summary>
        /// <param name="path">文件的物理路径。</param>
        /// <param name="text"></param>
        /// <param name="share">文件共享选项。</param>
        /// <returns>返回写入任务实例对象。</returns>
        public static async Task SaveTextAsync(string path, string text, FileShare share = FileShare.None)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, share))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    await sw.WriteAsync(text);
                }
            }
        }

        /// <summary>
        /// 将文件流保存到文件中。
        /// </summary>
        /// <param name="stream">当前文件流。</param>
        /// <param name="path">文件的物理路径。</param>
        /// <param name="share">文件共享选项。</param>
        /// <returns>返回保存任务。</returns>
        public static async Task SaveToAsync(this Stream stream, string path, FileShare share = FileShare.None)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, share))
            {
                var size = 409600;
                var buffer = new byte[size];
                var current = await stream.ReadAsync(buffer, 0, size);
                while (current > 0)
                {
                    await fs.WriteAsync(buffer, 0, current);
                    current = await stream.ReadAsync(buffer, 0, size);
                }
            }
        }
    }
}