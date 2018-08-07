namespace Mozlite.Mvc.Templates
{
    /// <summary>
    /// 声明语法。
    /// </summary>
    public class DeclaringSyntax
    {
        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// 声明字符串。
        /// </summary>
        public string Declare { get; internal set; }

        /// <summary>
        /// 返回当前声明语法的字符串。
        /// </summary>
        /// <returns>返回声明语法字符串。</returns>
        public override string ToString()
        {
            return $"<!{Name} {Declare}>";
        }
    }
}