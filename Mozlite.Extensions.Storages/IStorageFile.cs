namespace Mozlite.Extensions.Storages
{
    /// <summary>
    /// 存储文件。
    /// </summary>
    public interface IStorageFile
    {
        /// <summary>
        /// 大小。
        /// </summary>
        long Length { get; }

        /// <summary>
        /// 包含文件夹和文件名全名。
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// 文件名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 扩展名称。
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// 文件哈希值，一般为Md5值。
        /// </summary>
        string Hashed { get; }
    }
}