namespace Mozlite.Extensions.Storages
{
    /// <summary>
    /// 存储物理路径文件。
    /// </summary>
    public class StoredPhysicalFile
    {
        /// <summary>
        /// 文件名。
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 物理路径。
        /// </summary>
        public string PhysicalPath { get; set; }

        /// <summary>
        /// 内容类型。
        /// </summary>
        public string ContentType { get; set; }
    }
}