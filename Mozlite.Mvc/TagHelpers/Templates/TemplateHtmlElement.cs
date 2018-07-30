using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Mozlite.Mvc.TagHelpers.Templates
{
    /// <summary>
    /// Html节点。
    /// </summary>
    public class TemplateHtmlElement : TemplateElement
    {
        private readonly Dictionary<string, TemplateHtmlCodeAttribute> _attributes = new Dictionary<string, TemplateHtmlCodeAttribute>();
        private readonly Dictionary<string, TemplateHtmlAttribute> _basics = new Dictionary<string, TemplateHtmlAttribute>();
        /// <summary>
        /// 初始化类<see cref="TemplateHtmlElement"/>。
        /// </summary>
        /// <param name="tagName">标签名称。</param>
        /// <param name="position">位置。</param>
        protected internal TemplateHtmlElement(string tagName, int position) : base(position, TemplateElementType.Html)
        {
            TagName = tagName;
        }

        /// <summary>
        /// 获取当前名称的属性实例。
        /// </summary>
        /// <param name="name">属性名称。</param>
        /// <returns>返回属性实例对象。</returns>
        public TemplateHtmlAttribute this[string name]
        {
            get
            {
                if (_attributes.TryGetValue(name, out var value))
                    return value;
                if (_basics.TryGetValue(name, out var basic))
                    return basic;
                return null;
            }
            set
            {
                var attribute = value;
                if (attribute != null)
                    attribute.Element = this;
                if (attribute is TemplateHtmlCodeAttribute code)
                    _attributes[name] = code;
                else
                    _basics[name] = attribute;
            }
        }

        /// <summary>
        /// 标记名称。
        /// </summary>
        public string TagName { get; }

        /// <summary>
        /// 是否为自闭合节点。
        /// </summary>
        public bool IsSelfClosed { get; set; }

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("<").Append(TagName);
            if (_basics.Count > 0)
                sb.Append(" ").Append(string.Join(" ", _basics.Values.Where(x => !string.IsNullOrWhiteSpace(x.Value)).Select(x => $"{x.Name}=\"{x.Value.Replace("'", "\\'")}\"")));
            if (IsSelfClosed)
            {
                sb.AppendLine("/>");
                return sb.ToString();
            }
            sb.AppendLine(">");
            sb.Append(base.ToString());
            sb.AppendFormat("</{0}>", TagName).AppendLine();
            return sb.ToString();
        }

        /// <summary>
        /// 生成脚本。
        /// </summary>
        /// <param name="executor">脚本语法解析器。</param>
        /// <returns>返回生成后的脚本。</returns>
        public override string ToJsString(ITemplateExecutor executor)
        {
            var sb = new StringBuilder();
            AppendStartJsString(sb, executor);
            if (IsSelfClosed) return sb.ToString();
            foreach (var node in this)
            {
                sb.Append(node.ToJsString(executor));
            }
            sb.Append("html+='").Append("</").Append(TagName).Append(">';");
            return sb.ToString();
        }

        /// <summary>
        /// 生成HTML代码。
        /// </summary>
        /// <param name="executor">语法解析器。</param>
        /// <param name="instance">当前实例。</param>
        /// <param name="func">获取实例属性值。</param>
        /// <returns>返回HTML代码。</returns>
        public override string ToHtmlString(ITemplateExecutor executor, object instance, Func<object, string, object> func)
        {
            var sb = new StringBuilder();
            sb.Append("<").Append(TagName);
            if (_basics.Count > 0 || _attributes.Count > 0)
            {
                var result = _basics.ToDictionary(x => x.Key, x => _regex.Replace(x.Value.Value, match => func(instance, match.Groups[1].Value.Trim())?.ToString()));
                executor.Execute(_attributes.Values, result, instance, func);
                foreach (var basic in result)
                {
                    sb.Append(" ")
                        .Append(basic.Key)
                        .Append("=\"")
                        .Append(basic.Value)
                        .Append("\"");
                }
            }
            if (IsSelfClosed)
            {
                sb.Append("/>");
                return sb.ToString();
            }
            sb.Append(">");
            foreach (var node in this)
            {
                sb.Append(node.ToHtmlString(executor, instance, func));
            }
            sb.Append("</").Append(TagName).Append(">");
            return sb.ToString();
        }


        private static readonly Regex _regex = new Regex("{{(.*?)}}", RegexOptions.Singleline);
        private void AppendStartJsString(StringBuilder sb, ITemplateExecutor executor)
        {
            sb.Append("html+='<").Append(TagName);
            if (_basics.Count > 0 || _attributes.Count > 0)
            {
                var basics = _basics.ToDictionary(x => x.Key, x => _regex.Replace(x.Value.Value, "'+$1+'"));
                var js = executor.Execute(_attributes.Values, basics);
                foreach (var basic in basics)
                {
                    sb.Append(" ")
                        .Append(basic.Key)
                        .Append("=\"")
                        .Append(basic.Value)
                        .Append("\"");
                }
                if (!string.IsNullOrWhiteSpace(js))
                {
                    sb.Append("';");
                    sb.Append(js);
                    sb.Append("html+='");
                }
            }
            if (IsSelfClosed)
                sb.Append("/");
            sb.Append(">';");
        }
    }
}