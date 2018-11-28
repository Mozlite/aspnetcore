using Microsoft.AspNetCore.Razor.TagHelpers;
using Mozlite.Mvc.Messages;
using System;
using System.Linq;

namespace Mozlite.Mvc.TagHelpers.Bootstrap
{
    /// <summary>
    /// 状态消息。
    /// </summary>
    [HtmlTargetElement("moz:status-message-panel")]
    public class StatusMessageTagHelper : ViewContextableTagHelperBase
    {
        /// <summary>
        /// 是否有关闭按钮。
        /// </summary>
        [HtmlAttributeName("close")]
        public bool IsClose { get; set; }

        /// <summary>
        /// 是否弹窗。
        /// </summary>
        [HtmlAttributeName("modal")]
        public bool IsModal { get; set; }

        private bool IsRendered
        {
            get
            {
                if (HttpContext.Items.TryGetValue(typeof(StatusMessage), out var value) && value is bool rendered)
                    return rendered;
                return false;
            }
            set => HttpContext.Items[typeof(StatusMessage)] = value;
        }

        /// <summary>
        /// 访问并呈现当前标签实例。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        /// <param name="output">当前标签输出实例，用于呈现标签相关信息。</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (IsRendered)
            {//保证每个页面只有一个状态显示。
                output.SuppressOutput();
                return;
            }
            var message = new StatusMessage(ViewContext.TempData);
            if (message.Message == null)
            {
                var errorMessage = ViewContext.ModelState[String.Empty]?.Errors.FirstOrDefault()?.ErrorMessage;
                if (errorMessage == null)
                {
                    output.SuppressOutput();
                    return;
                }
                message.Set(StatusType.Danger, errorMessage);
            }

            var type = message.Type.ToString().ToLower();
            if (IsModal)
                output.Render("script", builder =>
                {
                    var json = new { message = message.Message, type };
                    builder.InnerHtml.AppendHtml("$(function(){").AppendHtml($"Mozlite.alert({json.ToJsonString()});").AppendHtml("});");
                });
            else
                output.Render("div", builer =>
                {
                    builer.AddCssClass("alert alert-dismissible");
                    builer.AddCssClass("alert-" + type);
                    builer.MergeAttribute("role", "alert");
                    if (IsClose)
                        builer.InnerHtml.AppendHtml(
                            "<button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>");
                    if (message.Type == StatusType.Success)
                        builer.InnerHtml.AppendHtml("<i class=\"fa fa-check\"></i>");
                    else
                        builer.InnerHtml.AppendHtml("<i class=\"fa fa-warning\"></i>");
                    builer.InnerHtml.AppendHtml(" ").AppendHtml(message.Message);
                });
            IsRendered = true;
        }
    }
}