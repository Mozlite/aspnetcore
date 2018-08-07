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
            return null;
        }
    }
}