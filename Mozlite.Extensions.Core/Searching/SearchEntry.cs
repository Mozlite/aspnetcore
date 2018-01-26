using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Mozlite.Extensions.Searching
{
    /// <summary>
    /// 搜索实体。
    /// </summary>
    public class SearchEntry
    {
        /// <summary>
        /// 实体唯一Id。
        /// </summary>
        public int Id { get; }

        private readonly IDictionary<string, string> _entries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        internal SearchEntry(DbDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                if (name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                    Id = reader.GetInt32(i);
                else
                    _entries.Add(name, GetString(reader.GetValue(i)));
            }
        }

        private string GetString(object value)
        {
            if (value == DBNull.Value)
                return null;
            return value.ToString().Trim();
        }

        /// <summary>
        /// 获取或设置实体实例对象。
        /// </summary>
        /// <param name="key">实体名称。</param>
        /// <returns>返回实体实例对象。</returns>
        public string this[string key]
        {
            get
            {
                string value;
                _entries.TryGetValue(key, out value);
                return value;
            }
            set { _entries[key] = value; }
        }
    }
}