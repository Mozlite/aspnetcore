namespace Mozlite.Data
{
    /// <summary>
    /// 数据源选项。
    /// </summary>
    public class DatabaseOptions
    {
        /// <summary>
        /// 链接字符串。
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 表格前缀。
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 表格架构。
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// 提供者。
        /// </summary>
        public string Provider { get; set; }
    }
}