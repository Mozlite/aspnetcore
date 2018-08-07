using System.Threading.Tasks;

namespace Mozlite.Mvc.Templates
{
    /// <summary>
    /// 语法。
    /// </summary>
    public interface ISyntax : IServices
    {
        /// <summary>
        /// 名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 执行当前语法。
        /// </summary>
        /// <param name="syntax">语法实例。</param>
        /// <returns>返回当前语法生成的代码字符串。</returns>
        Task<string> ExecuteAsync(Syntax syntax);
    }
}