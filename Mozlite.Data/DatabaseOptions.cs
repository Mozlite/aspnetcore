using System.Collections.Generic;

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
        /// 提供者。
        /// </summary>
        public string Provider { get; set; }

        private readonly IDictionary<string, string> _configs = new Dictionary<string, string>();
        /// <summary>
        /// 获取或设置数据库选项配置。
        /// </summary>
        /// <param name="key">配置名称。</param>
        public string this[string key]
        {
            get
            {
                _configs.TryGetValue(key, out var value);
                return value;
            }
            set => _configs[key] = value;
        }
    }
}