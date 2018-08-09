namespace Mozlite.Mvc.Templates
{
    /// <summary>
    /// 语法管理接口。
    /// </summary>
    public interface ISyntaxManager : ISingletonService
    {
        /// <summary>
        /// 解析模板语法。
        /// </summary>
        /// <param name="source">模板源代码。</param>
        /// <returns>返回当前文档实例。</returns>
        DocumentSyntax Parse(string source);

        /// <summary>
        /// 加载模板文件。
        /// </summary>
        /// <param name="path">文件物理路径。</param>
        /// <returns>返回当前文档实例。</returns>
        DocumentSyntax Load(string path);
    }
}