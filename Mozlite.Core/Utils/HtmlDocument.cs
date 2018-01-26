using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Mozlite.Utils
{
    /// <summary>
    /// HTML文档。
    /// </summary>
    public class HtmlDocument : HtmlSource
    {
        private readonly string _source;
        /// <summary>
        /// 初始化类<see cref="HtmlDocument"/>。
        /// </summary>
        /// <param name="source">HTML代码。</param>
        public HtmlDocument(string source) : base(source)
        {
            _source = source?.Trim();
            _metas = new Lazy<IDictionary<string, string>>(LoadMetas);
        }

        private IDictionary<string, string> LoadMetas()
        {
            var metas = new Dictionary<string, string>();
            var source = _source;
            var index = source.IndexOf("<meta ", StringComparison.OrdinalIgnoreCase);
            if (index == -1)
                return metas;
            do
            {
                source = source.Substring(index + 6);
                index = source.IndexOf('>');
                if (index == -1)
                    break;
                var item = source.Substring(0, index);
                var name = _nameRegex.Match(item)?.Groups[1].Value.Trim();
                if (name == null)
                    continue;
                var content = _contentRegex.Match(item)?.Groups[1].Value.Trim();
                if (content == null)
                    continue;
                metas[name] = content;
                source = source.Substring(index + 1);
                index = source.IndexOf("<meta ", StringComparison.OrdinalIgnoreCase);
            } while (index > 0);
            return metas;
        }

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            return _source;
        }

        /// <summary>
        /// 获取HTML标题部分，不会截取字符串。
        /// </summary>
        /// <returns>返回HTML标题部分。</returns>
        public string GetTitle()
        {
            return _source.Substring("<title>", "</title>");
        }

        private static readonly Regex _nameRegex = new Regex("name=\"(.*?)\"", RegexOptions.IgnoreCase);
        private static readonly Regex _contentRegex = new Regex("content=\"(.*?)\"", RegexOptions.IgnoreCase);
        private readonly Lazy<IDictionary<string, string>> _metas;
        /// <summary>
        /// HTML得Meta标签键列表。
        /// </summary>
        public IEnumerable<string> MetaKeys => _metas.Value.Keys;

        /// <summary>
        /// 获取HTML的Meta标签内容。
        /// </summary>
        /// <param name="name">标签名称。</param>
        /// <param name="content">内容。</param>
        /// <returns>返回HTML的Meta标签内容。</returns>
        public bool TryGetMeta(string name, out string content)
        {
            return _metas.Value.TryGetValue(name, out content);
        }
    }
}