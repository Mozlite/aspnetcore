using System;
using System.Collections.Generic;

namespace Mozlite.Extensions.Html
{
    /// <summary>
    /// 模板配置。
    /// </summary>
    public class TemplateConfiguration
    {
        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 版本。
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 模板唯一ID。
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// MD5值。
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 作者。
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 描述。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 参数。
        /// </summary>
        public IDictionary<string, object> Parameters { get; set; }
    }
}