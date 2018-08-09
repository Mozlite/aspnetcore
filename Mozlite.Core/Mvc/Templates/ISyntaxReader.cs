namespace Mozlite.Mvc.Templates
{
    /// <summary>
    /// 语法读取接口。
    /// </summary>
    public interface ISyntaxReader : ISingletonServices
    {
        /// <summary>
        /// 读取名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 读取当前语法实例。
        /// </summary>
        /// <param name="reader">代码读取实例。</param>
        /// <returns>返回当前读取的语法实例。</returns>
        Syntax Read(CodeReader reader);
    }
}