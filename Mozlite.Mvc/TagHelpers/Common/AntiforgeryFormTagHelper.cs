using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mozlite.Mvc.TagHelpers.Common
{
    /// <summary>
    /// 添加默认Antiforgery表单，进行Ajax验证。
    /// </summary>
    [HtmlTargetElement("moz:form-antiforgery")]
    public class AntiforgeryFormTagHelper : FormTagHelper
    {
        private const string AntiforgeryId = "ajax-protected-form";
        private bool IsAntiforgeryRendered
        {
            get
            {
                if (ViewContext.HttpContext.Items.TryGetValue(AntiforgeryId, out var value) && value is bool rendered)
                    return rendered;
                return false;
            }
            set => ViewContext.HttpContext.Items[AntiforgeryId] = value;
        }

        /// <summary>
        /// 初始化类<see cref="AntiforgeryFormTagHelper"/>。
        /// </summary>
        /// <param name="generator"><see cref="IHtmlGenerator"/>实例。</param>
        public AntiforgeryFormTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

        /// <summary>
        /// 初始化当前标签上下文。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        public override void Init(TagHelperContext context)
        {
            Antiforgery = true;
            base.Init(context);
        }

        /// <summary>
        /// 异步访问并呈现当前标签实例。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        /// <param name="output">当前标签输出实例，用于呈现标签相关信息。</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (IsAntiforgeryRendered)
            {//保证每个页面只有一个状态显示。
                output.SuppressOutput();
                return;
            }

            output.TagName = "form";
            base.Process(context, output);
            output.Attributes.SetAttribute("id", AntiforgeryId);
        }
    }
}