using System;
using System.Collections;
using System.Collections.Generic;

namespace Mozlite.Data.Migrations
{
    /// <summary>
    /// 操作命令。
    /// </summary>
    public class MigrationCommandListBuilder : IEnumerable<string>
    {
        private IndentedStringBuilder _builder = new IndentedStringBuilder();
        private readonly List<string> _commands = new List<string>();
        /// <summary>
        /// 结束命令。
        /// </summary>
        /// <returns>返回当前脚本。</returns>
        public void EndCommand()
        {
            _commands.Add(_builder.ToString());
            _builder = new IndentedStringBuilder();
        }

        /// <summary>
        /// 添加命令字符串。
        /// </summary>
        /// <param name="command">命令字符串。</param>
        /// <returns>返回当前命令列表构建实例。</returns>
        public MigrationCommandListBuilder Append(string command)
        {
            _builder.Append(command);
            return this;
        }

        /// <summary>
        /// 添加命令字符串。
        /// </summary>
        /// <param name="command">命令字符串。</param>
        /// <returns>返回当前命令列表构建实例。</returns>
        public MigrationCommandListBuilder AppendLine(string command)
        {
            _builder.AppendLine(command);
            return this;
        }

        /// <summary>
        /// 添加命令字符串。
        /// </summary>
        /// <returns>返回当前命令列表构建实例。</returns>
        public MigrationCommandListBuilder AppendLine()
        {
            _builder.AppendLine();
            return this;
        }

        /// <summary>返回一个循环访问集合的枚举器。</summary>
        /// <returns>用于循环访问集合的枚举数。</returns>
        public IEnumerator<string> GetEnumerator()
        {
            if (_builder != null)
            {
                var builder = _builder.ToString();
                if (!string.IsNullOrWhiteSpace(builder))
                    EndCommand();
                _builder = null;
            }
            return _commands.GetEnumerator();
        }

        /// <summary>返回循环访问集合的枚举数。</summary>
        /// <returns>可用于循环访问集合的 <see cref="T:System.Collections.IEnumerator" /> 对象。</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 增加缩进块。
        /// </summary>
        /// <returns>返回当前块实例，使用using将自动释放。</returns>
        public IDisposable Indent()
        {
            return _builder.Indent();
        }
    }
}