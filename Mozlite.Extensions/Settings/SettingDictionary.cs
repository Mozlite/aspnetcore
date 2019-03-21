using Mozlite.Extensions.Groups;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Settings
{
    /// <summary>
    /// 字典类型。
    /// </summary>
    [Table("core_Settings_Dictionary")]
    public class SettingDictionary : GroupBase<SettingDictionary>
    {
        /// <summary>
        /// 值。
        /// </summary>
        [Size(256)]
        public string Value { get; set; }

        private string _path;
        /// <summary>
        /// 路径，以“.”分割父级名称。
        /// </summary>
        public string Path
        {
            get
            {
                if (_path == null)
                {
                    var list = new List<string>();
                    var current = this;
                    while (current?.Id > 0)
                    {
                        list.Add(current.Name);
                        current = current.Parent;
                    }
                    list.Reverse();
                    _path = string.Join(".", list).ToLower();
                }

                return _path;
            }
        }

        /// <summary>
        /// 隐士转换为字符串。
        /// </summary>
        /// <param name="dic">当前字典实例。</param>
        public static implicit operator string(SettingDictionary dic) => dic?.Value;
    }
}