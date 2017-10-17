using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace Mozlite.Extensions.Tasks
{
    /// <summary>
    /// 参数实例对象。
    /// </summary>
    public class Argument : IEnumerable<string>
    {
        private const string Splitter = "=a=r=g=u=\r\n=m=e=n=t=";
        private readonly List<string> _arguments = new List<string>();

        /// <summary>
        /// 返回一个循环访问集合的枚举器。
        /// </summary>
        /// <returns>
        /// 可用于循环访问集合的 <see cref="T:System.Collections.Generic.IEnumerator`1"/>。
        /// </returns>
        public IEnumerator<string> GetEnumerator()
        {
            return _arguments.GetEnumerator();
        }

        /// <summary>
        /// 返回一个循环访问集合的枚举器。
        /// </summary>
        /// <returns>
        /// 可用于循环访问集合的 <see cref="T:System.Collections.IEnumerator"/> 对象。
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 索引查找和设置参数实例对象。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string this[int index]
        {
            get => _arguments[index];
            set => _arguments[index] = value;
        }

        /// <summary>
        /// 返回表示当前对象的字符串。
        /// </summary>
        /// <returns>
        /// 表示当前对象的字符串。
        /// </returns>
        public override string ToString()
        {
            return string.Join(Splitter, _arguments);
        }

        /// <summary>
        /// 显示字符串，每一行一个参数。
        /// </summary>
        /// <returns>返回显示字符串。</returns>
        public IHtmlContent ToHtmlString()
        {
            var args = string.Join(", ", _arguments.Select(arg => $"\"{arg.Replace("\"", "\\\"")}\""));
            return new HtmlString($"({args})");
        }

        /// <summary>
        /// 隐式转换<see cref="Argument"/>。
        /// </summary>
        /// <param name="arguments">参数字符串。</param>
        public static implicit operator Argument(string arguments)
        {
            var args = new Argument();
            args._arguments.AddRange(arguments.Split(new[] { Splitter }, StringSplitOptions.None));
            return args;
        }

        /// <summary>
        /// 隐式转换<see cref="Argument"/>。
        /// </summary>
        /// <param name="arguments">参数列表。</param>
        public static implicit operator Argument(List<string> arguments)
        {
            var args = new Argument();
            args._arguments.AddRange(arguments);
            return args;
        }

        /// <summary>
        /// 隐式转换<see cref="Argument"/>。
        /// </summary>
        /// <param name="arguments">参数列表。</param>
        public static implicit operator Argument(List<object> arguments)
        {
            var args = new Argument();
            args._arguments.AddRange(arguments.Select(a => a?.ToString()));
            return args;
        }

        /// <summary>
        /// 隐式转换<see cref="Argument"/>。
        /// </summary>
        /// <param name="arguments">参数列表。</param>
        public static implicit operator Argument(string[] arguments)
        {
            var args = new Argument();
            args._arguments.AddRange(arguments);
            return args;
        }

        /// <summary>
        /// 隐式转换<see cref="Argument"/>。
        /// </summary>
        /// <param name="arguments">参数列表。</param>
        public static implicit operator Argument(object[] arguments)
        {
            var args = new Argument();
            args._arguments.AddRange(arguments.Select(a => a?.ToString()));
            return args;
        }

        /// <summary>
        /// 参数个数。
        /// </summary>
        public int Count => _arguments.Count;

        /// <summary>
        /// 获取整形参数。
        /// </summary>
        /// <param name="index">索引值。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>返回当前参数。</returns>
        public int GetInt32(int index, int defaultValue = 0)
        {
            try
            {
                if (int.TryParse(this[index], out var value))
                    return value;
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        internal Func<Argument, Task> SetArgumentAsync;
        /// <summary>
        /// 保存当前参数实例。
        /// </summary>
        /// <returns>返回保存任务。</returns>
        public Task SaveAsync()
        {
            return SetArgumentAsync?.Invoke(this);
        }
    }
}