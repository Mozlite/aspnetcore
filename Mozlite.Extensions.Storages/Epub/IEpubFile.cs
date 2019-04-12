namespace Mozlite.Extensions.Storages.Epub
{
    /// <summary>
    /// Epub文件接口。
    /// </summary>
    public interface IEpubFile
    {
        /// <summary>
        /// 添加一个文本文件。
        /// </summary>
        /// <param name="fileName">文件名称。</param>
        /// <param name="content">文件内容。</param>
        /// <param name="title">标题，如果是文档文件需要标题。</param>
        /// <param name="isSpine">是否为文件档案。</param>
        void AddContent(string fileName, string content, string title, bool isSpine = true);

        /// <summary>
        /// 添加一个文件。
        /// </summary>
        /// <param name="fileName">文件名称。</param>
        /// <param name="path">物理路径。</param>
        void AddFile(string fileName, string path);

        /// <summary>
        /// 添加一个文件。
        /// </summary>
        /// <param name="fileName">文件名称。</param>
        /// <param name="path">物理路径。</param>
        /// <param name="title">标题，如果是文档文件需要标题。</param>
        void AddFile(string fileName, string path, string title);

        /// <summary>
        /// 添加封面图片。
        /// </summary>
        /// <param name="path">物理路径。</param>
        /// <param name="extension">图片扩展名。</param>
        /// <returns>返回封面路径。</returns>
        string AddCover(string path, string extension = ".png");

        /// <summary>
        /// 编译成Epub文件，并返回物理路径。
        /// </summary>
        /// <param name="overwrite">是否覆盖已有文件。</param>
        /// <returns>返回当前文件得物理路径。</returns>
        string Compile(bool overwrite);
    }
}