using System;
using System.Collections.Generic;

namespace Mozlite.Extensions.Messages
{
    /// <summary>
    /// 扩展类。
    /// </summary>
    public static class MessageExtensions
    {
        /// <summary>
        /// 替换关键词。
        /// </summary>
        /// <param name="message">当前消息。</param>
        /// <param name="action">添加关键词。</param>
        /// <returns>返回替换后的字符串。</returns>
        public static string ReplaceBy(this string message, Action<IDictionary<string, string>> action)
        {
            var dic = new Dictionary<string, string>();
            action(dic);
            foreach (var kw in dic)
            {
                message = message.Replace($"[${kw.Key};]", kw.Value);
            }
            return message;
        }
    }
}