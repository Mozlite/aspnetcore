namespace Mozlite.Mvc.Templates.Declarings
{
    /// <summary>
    /// 声明语法。
    /// </summary>
    public interface IDeclareSyntax : IServices
    {
        /// <summary>
        /// 名称。
        /// </summary>
        string Name { get; }
    }
}