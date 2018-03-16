using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mozlite.Mvc.TagHelpers.Common
{
    /// <summary>
    /// 样式标签。
    /// </summary>
    [HtmlTargetElement("*", Attributes = ClassValuesPrefix + "*")]
    public class ClassTagHelper : TagHelperBase
    {
        private const string ClassValuesPrefix = ".class-";
        private const string ClassValuesDictionaryName = ".class-data";
        private IDictionary<string, bool> _classNames;

        /// <summary>
        /// 样式列表。
        /// </summary>
        [HtmlAttributeName(ClassValuesDictionaryName, DictionaryAttributePrefix = ClassValuesPrefix)]
        public IDictionary<string, bool> ClassNames
        {
            get
            {
                if (_classNames == null)
                {
                    _classNames = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
                }

                return _classNames;
            }
            set => _classNames = value;
        }

        /// <summary>
        /// 访问并呈现当前标签实例。
        /// </summary>
        /// <param name="context">当前HTML标签上下文，包含当前HTML相关信息。</param>
        /// <param name="output">当前标签输出实例，用于呈现标签相关信息。</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var classNames = ClassNames.Where(x => x.Value).Select(x => x.Key).ToArray();
            output.AddCssClass(classNames);
        }
    }
}