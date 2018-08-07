using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mozlite.Extensions.Documents.Html
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

        private IDictionary<string, object> _tempData;
        /// <summary>
        /// 转换为临时对象。
        /// </summary>
        /// <returns>返回临时对象实例。</returns>
        [DebuggerStepThrough]
        internal IDictionary<string, object> ToTempData()
        {
            if (_tempData == null)
            {
                _tempData = new Dictionary<string, object>();
                _tempData["Id"] = Id;
                _tempData["Name"] = Name;
                _tempData["Version"] = Version;
                _tempData["Author"] = Author;
                _tempData["Description"] = Description;
                if (Parameters != null)
                {
                    foreach (var parameter in Parameters)
                    {
                        _tempData[$"P_{parameter.Key}"] = parameter.Value;
                    }
                }
            }
            return _tempData;
        }
    }
}