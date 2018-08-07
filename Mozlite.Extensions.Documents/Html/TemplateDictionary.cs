using System;
using System.Collections.Generic;

namespace Mozlite.Extensions.Documents.Html
{
    /// <summary>
    /// 模板对象存储。
    /// </summary>
    public class TemplateDictionary : Dictionary<string, object>
    {
        /// <summary>
        /// 初始化类<see cref="TemplateDictionary"/>。
        /// </summary>
        public TemplateDictionary() : base(StringComparer.OrdinalIgnoreCase) { }
    }
}