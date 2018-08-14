namespace Mozlite.Mvc.Templates.Codings
{
    /// <summary>
    /// 代码语法。
    /// </summary>
    public class CodeSyntax : Syntax
    {
        /// <summary>
        /// 当前语法的呈现字符串。
        /// </summary>
        /// <returns>返回当前语法的呈现字符串。</returns>
        public override string ToString()
        {
            return $"{Indent()}{Name}\r\n";
        }
    }
}