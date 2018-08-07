using System.Text;

namespace Mozlite.Mvc.Templates
{
    /// <summary>
    /// 文档语法。
    /// </summary>
    public class DocumentSyntax : Syntax
    {
        /// <summary>
        /// 当前语法的呈现字符串。
        /// </summary>
        /// <returns>返回当前语法的呈现字符串。</returns>
        public override string ToString()
        {
            return this.Join("\r\n");
        }
    }
}