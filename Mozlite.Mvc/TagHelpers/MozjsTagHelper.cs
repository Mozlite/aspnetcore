using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mozlite.Mvc.TagHelpers
{
    /// <summary>
    /// js脚本标签，可以和HTML混合并用，开放参数：
    /// $this:表示当前标签jQuery对象；
    /// $model:表示当前JSON对象；
    /// $render:呈现对象方法，参数为当前实例；
    /// </summary>
    [HtmlTargetElement("moz:js", Attributes = HrefAttributeName)]
    [HtmlTargetElement("moz:js", Attributes = JsonAttributeName)]
    [HtmlTargetElement("script", Attributes = "[type=mozjs]")]
    public class MozjsTagHelper : ViewContextableTagHelperBase
    {
        private const string HrefAttributeName = "href";
        private const string JsonAttributeName = "json";

        /// <summary>
        /// 获取JSON对象的地址。
        /// </summary>
        [HtmlAttributeName(HrefAttributeName)]
        public string Url { get; set; }

        /// <summary>
        /// 获取JSON对象的地址。
        /// </summary>
        [HtmlAttributeName(JsonAttributeName)]
        public string Json { get; set; }

        /// <summary>
        /// 下一次获取时间。
        /// </summary>
        [HtmlAttributeName("interval")]
        public int Interval { get; set; }

        /// <summary>
        /// 当前标签名称。
        /// </summary>
        [HtmlAttributeName("tag")]
        public string TagName { get; set; } = "div";

        /// <summary>
        /// 发送的数据。
        /// </summary>
        [HtmlAttributeName("js", DictionaryAttributePrefix = "js-")]
        public IDictionary<string, string> Data { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 异步访问并呈现当前标签实例。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        /// <param name="output">当前标签输出实例，用于呈现标签相关信息。</param>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var content = await output.GetChildContentAsync();
            if (content.IsEmptyOrWhiteSpace)
            {
                output.SuppressOutput();
                return;
            }
            output.TagName = TagName;
            var id = "mozjs-" + GetCounter();
            output.AddCssClass(id);
            output.Content.AppendHtml("<script>");
            output.Content.Append("$(function () {var $this = $('." + id + "');");
            if (Url == null)
                output.Content.AppendFormat("c({0});", Json);
            else
            {
                if (Url.IndexOf('?') == -1)
                    Url += "?_=";
                else
                    Url += "&_=";
                var interval = 5000;
                if (Interval > 0)
                    interval = Interval * 1000;
                output.Content.Append("(function b() {$.ajax({ url: '" + Url);
                output.Content.Append("'+(+new Date()),dataType: 'JSON', type: 'GET', success:function (e) {$render(e);if (a) clearTimeout(a);a = setTimeout(b, " + interval + ");},");
                output.Content.Append("error:function () {if (a) clearTimeout(a);a = setTimeout(b, " + interval + ");}});})();");
            }

            var source = content.ToString().Trim();
            output.Content.Append("var d;function w(s) { d.push(s);}function $render($model) {d = [];");
            Process(output.Content, source);
            output.Content.Append(
                "$this.html(d.join(''));}});");
            output.Content.AppendHtml("</script>");
        }

        private void Process(TagHelperContent output, string source)
        {
            var reader = new SourceReader(source);
            var buffer = new StringBuilder();

            while (reader.MoveNext())
            {
                if (reader.Current == '<' && !reader.IsNext(' '))
                {//读取html标签
                    ReadHtml(output, buffer, reader);
                }

                var code = reader.ReadUntil(new[] {'`', '<'});
                output.Append(code);
                reader.Skip();
            }
        }

        private void ReadHtml(TagHelperContent output, StringBuilder builder, SourceReader reader)
        {
            while (reader.Current != SourceReader.NullChar)
            {
                if (reader.Current == '$' && reader.IsNext('{'))
                {//代码块
                    Queue(output, builder);
                    ReadInlineCode(output, reader);
                }

                if (reader.Current == '>')
                {//标签结束
                    builder.Append(reader.Current);
                    Queue(output, builder);
                    reader.Skip();
                    reader.MoveNext();
                    return;
                }
                builder.Append(reader.Current);
                reader.Skip();
            }
        }

        private void Queue(TagHelperContent output, StringBuilder builder)
        {
            if (builder.Length == 0)
                return;
            output.Append("w('").Append(builder.ToString()).Append("');");
            builder.Clear();
        }

        private void ReadInlineCode(TagHelperContent output, SourceReader reader)
        {
            reader.Skip(2);
            output.Append("w(");
            output.Append(reader.ReadUntil('}'));
            output.Append(");");
            reader.Skip();
        }
    }
}