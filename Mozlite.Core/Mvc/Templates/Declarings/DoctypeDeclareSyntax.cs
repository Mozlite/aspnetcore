using System.IO;

namespace Mozlite.Mvc.Templates.Declarings
{
    /// <summary>
    /// 声明语法。
    /// !doctype
    /// </summary>
    public class DoctypeDeclareSyntax : DeclareSyntax
    {
        /// <summary>
        /// 名称。
        /// </summary>
        public override string Name => "doctype";
        
        /// <summary>
        /// 解析当前声明，并写入到实例对象中。
        /// </summary>
        /// <param name="writer">字符串写入器。</param>
        /// <param name="declare">声明的字符串。</param>
        public override void Write(TextWriter writer, string declare)
        {
            declare = declare ?? "html";
            writer.Write($"<!DOCTYPE {declare}>");
        }
    }
}