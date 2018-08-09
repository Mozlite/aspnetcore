using System.Collections.Generic;

namespace Mozlite.Mvc.Templates
{
    /// <summary>
    /// 声明语法。
    /// </summary>
    public class Declaring
    {
        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 声明字符串。
        /// </summary>
        public string Declare { get; set; }

        /// <summary>
        /// 参数。
        /// </summary>
        public List<string> Parameters { get; set; }

        /// <summary>
        /// 特性。
        /// </summary>
        public IDictionary<string, string> Attributes { get; set; }

        /// <summary>
        /// 声明类型。
        /// </summary>
        public DeclaringType DeclaringType { get; set; }
    }
}