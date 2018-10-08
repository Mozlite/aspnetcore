using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite.Utils;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mozlite.Mvc.TagHelpers.Common
{
    /// <summary>
    /// js脚本标签，可以和HTML混合并用，开放参数：
    /// $this:表示当前标签jQuery对象；
    /// $model:表示当前JSON对象；
    /// 注意：script标签里的代码将在呈现后进行调用。
    /// </summary>
    [HtmlTargetElement("*", Attributes = "mozjs")]
    [HtmlTargetElement("moz:js", Attributes = "href")]
    [HtmlTargetElement("moz:js", Attributes = "json")]
    public class MozjsTagHelper : ViewContextableTagHelperBase
    {
        /// <summary>
        /// 子字符串为mozjs语法：HTML和js混合语法，开放参数：
        /// $this:表示当前标签jQuery对象；
        /// $model:表示当前JSON对象；
        /// 注意：script标签里的代码将在呈现后进行调用。
        /// </summary>
        [HtmlAttributeName("mozjs")]
        public bool Mozjs { get; set; }

        /// <summary>
        /// 获取JSON对象的地址。
        /// </summary>
        [HtmlAttributeName("href")]
        public string Url { get; set; }

        /// <summary>
        /// 获取JSON对象的地址。
        /// </summary>
        [HtmlAttributeName("json")]
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
        public string TagName { get; set; }

        /// <summary>
        /// 异步访问并呈现当前标签实例。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        /// <param name="output">当前标签输出实例，用于呈现标签相关信息。</param>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var content = await output.GetChildContentAsync();
            if (content.IsEmptyOrWhiteSpace || !Mozjs)
            {
                output.SuppressOutput();
                return;
            }
            if (TagName != null)
                output.TagName = TagName;
            var id = "mozjs-" + GetCounter();
            output.AddCssClass(id);
            output.Content.AppendHtml("<script>");
            output.Content.AppendHtml("$(function () {var $this = $('." + id + "'), a, d;");
            if (Url == null)
                output.Content.AppendHtml("c(")
                    .AppendHtml(Json)
                    .AppendHtml(");");
            else
            {
                if (Url.IndexOf('?') == -1)
                    Url += "?_=";
                else
                    Url += "&_=";
                output.Content
                    .AppendHtml("(function b() {$.ajax({ url: '")
                    .AppendHtml(Url)
                    .AppendHtml("'+(+new Date()),dataType: 'JSON', type: 'GET', success:function (e) {d = [];r(e);");
                SetTimeout(output.Content);
                output.Content.AppendHtml("},").AppendHtml("error:function () {");
                SetTimeout(output.Content);
                output.Content.AppendHtml("}});})();");
            }

            var source = content.GetContent();
            var scripts = new StringBuilder();
            //提取脚本
            source = _regex.Replace(source, match =>
            {
                var script = match.Groups[1].Value.Trim().Trim(';');
                script = _whiteSpace.Replace(script, "$1");
                scripts.Append(script).Append(";");
                return null;
            }).Trim();
            output.Content.AppendHtml("function _(s) { d.push(s);}function r($model) {");
            Process(output.Content, source);
            output.Content.AppendHtml("$this.html(d.join(''));");
            output.Content.AppendHtml("if(Mozlite&&Mozlite.render)Mozlite.render($this);");
            output.Content.AppendHtml(scripts.ToString()).AppendHtml("}});");
            output.Content.AppendHtml("</script>");
        }

        private static readonly Regex _whiteSpace = new Regex("([{;}])(\\r)?\\n\\s*", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex _regex = new Regex("<script.*?>(.*?)</script>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private void SetTimeout(TagHelperContent output)
        {
            if (Interval == 0) return;
            output.AppendHtml("if (a) clearTimeout(a);a = setTimeout(b, " + (Interval * 1000) + ");");
        }

        private void Process(TagHelperContent output, string source)
        {
            var reader = new SourceReader(source);
            var builder = new StringBuilder();

            while (reader.MoveNext())
            {
                if (reader.Current == '<' && !reader.IsNext(' '))
                {//读取html标签
                    ReadHtml(output, builder, reader);
                    continue;
                }

                if (reader.Current == '$' && reader.IsNextNonWhiteSpace('{'))
                {//代码块
                    Queue(output, builder);
                    ReadInlineCode(output, reader);
                    continue;
                }

                if (reader.Current == '`')
                {//字符串
                    ReadQuote(output, builder, reader);
                    continue;
                }

                var code = reader.ReadUntil(new[] { '`', '<' }).Trim();
                foreach (var s in code.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (string.IsNullOrWhiteSpace(s))
                        continue;
                    output.AppendHtml(s.Trim());
                }
            }
        }

        private void ReadHtml(TagHelperContent output, StringBuilder builder, SourceReader reader)
        {
            while (reader.Current != SourceReader.NullChar)
            {
                if (reader.Current == '$' && reader.IsNextNonWhiteSpace('{'))
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

        private void ReadQuote(TagHelperContent output, StringBuilder builder, SourceReader reader)
        {
            reader.Skip();
            while (reader.Current != SourceReader.NullChar)
            {
                if (reader.Current == '$' && reader.IsNextNonWhiteSpace('{'))
                {//代码块
                    Queue(output, builder);
                    ReadInlineCode(output, reader);
                }

                if (reader.Current == '`')
                {
                    if (reader.IsNext('`'))
                        reader.Skip();
                    else
                    {
                        //字符串结束
                        Queue(output, builder, ' ');
                        reader.Skip();
                        reader.MoveNext();
                        return;
                    }
                }

                builder.Append(reader.Current);
                reader.Skip();
            }
        }

        private void Queue(TagHelperContent output, StringBuilder builder, params char[] trimChars)
        {
            if (builder.Length == 0)
                return;
            var source = builder.ToString();
            if (trimChars != null)
                source = source.Trim(trimChars);
            foreach (var s in source.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (string.IsNullOrWhiteSpace(s))
                    continue;
                output.AppendHtml("_('").AppendHtml(s.Trim()).AppendHtml("');");
            }
            builder.Clear();
        }

        private void ReadInlineCode(TagHelperContent output, SourceReader reader)
        {
            output.AppendHtml("_(");
            output.AppendHtml(reader.ReadUntil('}').Trim());
            output.AppendHtml(");");
            reader.Skip();
        }
    }
}