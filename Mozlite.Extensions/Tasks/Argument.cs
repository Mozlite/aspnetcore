using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;

namespace Mozlite.Extensions.Tasks
{
    /// <summary>
    /// 参数实例对象。
    /// </summary>
    public class Argument
    {
        private readonly IDictionary<string, object> _arguments = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 初始化类<see cref="Argument"/>。
        /// </summary>
        public Argument() { }

        internal Argument(string arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments))
                return;
            var data = Cores.FromJsonString<Dictionary<string, object>>(arguments);
            foreach (var o in data)
            {
                _arguments[o.Key] = o.Value;
            }
        }

        /// <summary>
        /// 索引查找和设置参数实例对象。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <returns></returns>
        public object this[string name]
        {
            get
            {
                if (_arguments.TryGetValue(name, out var value))
                    return value;
                return null;
            }
            set => _arguments[name] = value;
        }

        /// <summary>
        /// 返回表示当前对象的字符串。
        /// </summary>
        /// <returns>
        /// 表示当前对象的字符串。
        /// </returns>
        public override string ToString()
        {
            return _arguments.ToJsonString();
        }

        /// <summary>
        /// 显示字符串，每一行一个参数。
        /// </summary>
        /// <returns>返回显示字符串。</returns>
        public IHtmlContent ToHtmlString()
        {
            return new HtmlString($"({ToString()})");
        }

        /// <summary>
        /// 参数个数。
        /// </summary>
        public int Count => _arguments.Count;

        /// <summary>
        /// 获取整形参数。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>返回当前参数。</returns>
        public int GetInt32(string name, int defaultValue = 0) => (int?)this[name] ?? defaultValue;

        /// <summary>
        /// 获取布尔型参数。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>返回当前参数。</returns>
        public bool GetBoolean(string name, bool defaultValue = false) => (bool?)this[name] ?? defaultValue;

        /// <summary>
        /// 当前服务Id。
        /// </summary>
        public TaskContext TaskContext { get; internal set; }

        /// <summary>
        /// 自定义后台服务运行模式。
        /// </summary>
        public string Interval { get => this[nameof(Interval)]?.ToString(); set => this[nameof(Interval)] = value; }

        /// <summary>
        /// 错误消息。
        /// </summary>
        public string Error { get => this[nameof(Error)]?.ToString(); set => this[nameof(Error)] = value; }

        /// <summary>
        /// 是否保存堆栈信息。
        /// </summary>
        public bool IsStack { get => GetBoolean(nameof(IsStack)); set => this[nameof(IsStack)] = value; }
    }
}