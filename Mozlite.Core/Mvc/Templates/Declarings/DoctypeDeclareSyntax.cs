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
        /// 返回解析后的代码。
        /// </summary>
        /// <returns>返回代码。</returns>
        public override string ToString()
        {
            return "<!DOCTYPE html>";
        }
    }
}