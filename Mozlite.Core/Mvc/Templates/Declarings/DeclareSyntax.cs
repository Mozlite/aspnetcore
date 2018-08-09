using System.IO;

namespace Mozlite.Mvc.Templates.Declarings
{
    /// <summary>
    /// 声明语法。
    /// </summary>
    public abstract class DeclareSyntax : IDeclareSyntax
    {
        private string _name;
        /// <summary>
        /// 名称。
        /// </summary>
        public virtual string Name => _name ?? (_name = GetType().Name.Replace("DeclareSyntax", null).ToLower());

        /// <summary>
        /// 返回解析后的代码。
        /// </summary>
        /// <returns>返回代码。</returns>
        public override string ToString()
        {
            return $"!{Name}";
        }

        /// <summary>
        /// 解析当前声明，并写入到实例对象中。
        /// </summary>
        /// <param name="writer">字符串写入器。</param>
        /// <param name="declare">声明的字符串。</param>
        public abstract void Write(TextWriter writer, string declare);
    }
}