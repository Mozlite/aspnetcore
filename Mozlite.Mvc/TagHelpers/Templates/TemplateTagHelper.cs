using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite.Extensions;
using Newtonsoft.Json;

namespace Mozlite.Mvc.TagHelpers.Templates
{
    /// <summary>
    /// 模板标签。
    /// </summary>
    [HtmlTargetElement("*", Attributes = "[moz-template=json]")]
    [HtmlTargetElement("*", Attributes = "[moz-template=data]")]
    public class TemplateTagHelper : TagHelper
    {
        private readonly ITemplateExecutor _executor;

        public TemplateTagHelper(ITemplateExecutor executor)
        {
            _executor = executor;
        }
        
        private IEntityType _entityType;

        /// <summary>
        /// 获取当前对象中<paramref name="propertyName"/>的值。
        /// </summary>
        /// <param name="model">模型实例。</param>
        /// <param name="propertyName">属性名称。</param>
        /// <returns>返回当前属性的值。</returns>
        protected object GetValue(object model, string propertyName)
        {
            if (propertyName == "$site")
                return (model as IEnumerable).OfType<object>().Count();
            _entityType = _entityType ?? model.GetType().GetEntityType();
            return _entityType?.FindProperty(propertyName)?.Get(model);
        }

        /// <summary>
        /// 绑定类型。
        /// </summary>
        [HtmlAttributeName("moz-template")]
        public string Binder { get; set; }

        /// <summary>
        /// 数据。
        /// </summary>
        [HtmlAttributeName("moz-models")]
        public object Models { get; set; }

        /// <summary>
        /// 远端数据。
        /// </summary>
        [HtmlAttributeName("moz-remote")]
        public string RemoteUrl { get; set; }

        /// <summary>
        /// 回掉方法。
        /// </summary>
        [HtmlAttributeName("moz-callback")]
        public string Callback { get; set; }

        /// <summary>
        /// 发送的数据。
        /// </summary>
        [HtmlAttributeName("moz-data", DictionaryAttributePrefix = "moz-data-")]
        public IDictionary<string, string> Data { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        
        /// <summary>
        /// 下一次获取数据的时间间隔（秒）。
        /// </summary>
        [HtmlAttributeName("moz-interval")]
        public int Interval { get; set; }

        /// <summary>
        /// 呈现脚本。
        /// </summary>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var template = (await output.GetChildContentAsync()).GetContent();
            if (string.IsNullOrWhiteSpace(template) || (Models == null && RemoteUrl == null))
            {
                output.SuppressOutput();
                return;
            }

            var document = new TemplateDocument(template);
            if (Binder == "json")
            {
                var id = context.AllAttributes["id"]?.Value;
                if (id == null)
                {
                    id = "moz_" + context.UniqueId;
                    output.Attributes.SetAttribute("id", id);
                }

                output.Content.AppendHtml("<script>");
                output.Content.AppendHtml("$(function(){");
                output.Content.AppendHtml($"var $this = $('#{id}');");
                Render(output, document);
                output.Content.AppendHtml(document.Scripts);
                output.Content.AppendHtml("});");
                output.Content.AppendHtml("</script>");
            }
            else
            {
                if (Models is IEnumerable instances)
                {
                    foreach (var instance in instances)
                    {
                        output.Content.AppendHtml(document.ToHtmlString(_executor, instance, GetValue));
                    }
                }
                else
                {
                    output.Content.AppendHtml(document.ToHtmlString(_executor, Models, GetValue));
                }
                if (!string.IsNullOrWhiteSpace(document.Scripts))
                    output.Content.AppendHtml("<script>").AppendHtml(document.Scripts).Append("</script>");
            }
        }

        private void Render(TagHelperOutput output, TemplateDocument document)
        {
            Func(output, "render", "data", c =>
            {
                c.AppendHtml("var html = '';");
                Func(output, "appender", "$model", ac => ac.AppendHtml(document.ToJsString(_executor)));
                c.AppendHtml("appender(data);");
                c.AppendHtml("$this.html(html);");
            });
            if (Models == null || Interval > 0)
            {
                Func(output, "ajax", "url", c =>
                {
                    c.AppendHtml("$.ajax({url:url+'&_'+(+new Date())")
                        .AppendHtml(", dataType:'JSON', type:'GET', success:function(data){")
                        .AppendHtml("render(data);");

                    if (Callback != null)
                        c.AppendHtml(Callback).AppendHtml("(data);");

                    if (Interval > 0)
                    {//定时器
                        c.AppendHtml("setTimeout(function(){ajax(url);}")
                            .AppendHtml(",")
                            .AppendHtml((Interval * 1000).ToString())
                            .AppendHtml(");");
                    }
                    c.AppendHtml("}, error:function(r){$this.html(r.responseText);");
                    if (Interval > 0)
                    {//定时器
                        c.AppendHtml("setTimeout(function(){ajax(url);}")
                            .AppendHtml(",")
                            .AppendHtml((Interval * 1000).ToString())
                            .AppendHtml(");");
                    }
                    c.AppendHtml("}});");
                });
            }

            switch (Models)
            {
                case string jscode when jscode.Trim().Length > 0:
                    output.Content.AppendHtml("render(");
                    output.Content.AppendHtml(jscode);
                    output.Content.AppendHtml(");");
                    break;
                default:
                    output.Content.AppendHtml("render(");
                    output.Content.AppendHtml(JsonConvert.SerializeObject(Models));
                    output.Content.AppendHtml(");");
                    break;
                case null:
                    output.Content.AppendHtml("ajax(");
                    var url = RemoteUrl;
                    if (url.IndexOf('?') == -1)
                        url += '?';
                    else
                        url += '&';
                    url += string.Join("&", Data.Select(x => $"{x.Key}={x.Value}"));
                    output.Content.AppendHtml("'").AppendHtml(url).AppendHtml("'");
                    output.Content.AppendHtml(");");
                    break;
            }
        }

        private void Func(TagHelperOutput output, string name, string args, Action<TagHelperContent> action)
        {
            output.Content.AppendHtml("function ");
            output.Content.AppendHtml(name);
            output.Content.AppendHtml("(");
            output.Content.AppendHtml(args);
            output.Content.AppendHtml(")");
            output.Content.AppendHtml("{");
            action(output.Content);
            output.Content.AppendHtml("};");
        }
    }
}