using Mozlite.Utils;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// 获取文件的编码格式。
        /// </summary>
        /// <param name="path">当前文件的物理路径。</param>
        /// <param name="defaultEncoding">默认编码，如果为<code>null</code>，则为<see cref="Encoding.Default"/>。</param>
        /// <returns>返回当前文件的编码。</returns>
        public static Encoding GetEncoding(string path, Encoding defaultEncoding = null)
        {
            defaultEncoding = defaultEncoding ?? Encoding.Default;
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                if (fs.Length < 3)
                    return defaultEncoding;
                var buffer = new byte[3];
                fs.Read(buffer, 0, 3);
                var unicode = new byte[] { 0xFF, 0xFE, 0x41 };
                var unicodeBig = new byte[] { 0xFE, 0xFF, 0x00 };
                var utf8 = new byte[] { 0xEF, 0xBB, 0xBF };//带BOM

                if (buffer[0] == utf8[0] && buffer[1] == utf8[1] && buffer[2] == utf8[2] || IsUTF8(fs))
                    return Encoding.UTF8;
                if (buffer[0] == unicodeBig[0] && buffer[1] == unicodeBig[1] && buffer[2] == unicodeBig[2])
                    return Encoding.BigEndianUnicode;
                if (buffer[0] == unicode[0] && buffer[1] == unicode[1] && buffer[2] == unicode[2])
                    return Encoding.Unicode;
                return defaultEncoding;
            }
        }

        /// <summary>
        /// 没有BOM。
        /// </summary>
        /// <param name="stream">文件流。</param>
        /// <returns>返回判断结果。</returns>
        private static bool IsUTF8(Stream stream)
        {
            var counter = 1;//计算当前正分析的字符应还有的字节数
            stream.Position = 0;
            var current = stream.ReadByte();
            while (current != -1)
            {
                if (counter == 1)
                {
                    if (current >= 0x80)
                    {
                        //判断当前
                        while (((current <<= 1) & 0x80) != 0)
                        {
                            counter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始，如：110XXXXX.....1111110X
                        if (counter == 1 || counter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((current & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    counter--;
                }
                current = stream.ReadByte();//当前分析的字节
            }
            if (counter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }

        /// <summary>
        /// 转换文件编码。
        /// </summary>
        /// <param name="directoryName">当前文件夹物理路径。</param>
        /// <param name="searchPattern">文件匹配模式。</param>
        /// <param name="option">检索选项。</param>
        /// <param name="defaultEncoding">默认编码，如果为<code>null</code>，则为<see cref="Encoding.Default"/>。</param>
        /// <param name="destinationEncoding">转换的编码，如果为<code>null</code>，则为<see cref="Encoding.UTF8"/>。</param>
        /// <returns>返回转换任务。</returns>
        public static async Task ConvertAsync(string directoryName, string searchPattern = "*.*", SearchOption option = SearchOption.AllDirectories, Encoding defaultEncoding = null, Encoding destinationEncoding = null)
        {
            var directory = new DirectoryInfo(directoryName);
            if (!directory.Exists)
                return;
            destinationEncoding = destinationEncoding ?? Encoding.UTF8;
            foreach (var info in directory.GetFiles(searchPattern, option))
            {
                var current = GetEncoding(info.FullName, defaultEncoding);
                if (current == destinationEncoding)
                    continue;
                var content = File.ReadAllText(info.FullName, current);
                using (var fs = new FileStream(info.FullName, FileMode.Create, FileAccess.Write))
                using (var writer = new StreamWriter(fs, destinationEncoding))
                    await writer.WriteAsync(content);
            }
        }
    }
}