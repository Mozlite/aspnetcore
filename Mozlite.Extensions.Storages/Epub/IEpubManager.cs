namespace Mozlite.Extensions.Storages.Epub
{
    /// <summary>
    /// Epub管理接口。
    /// </summary>
    public interface IEpubManager : ISingletonService
    {
        /// <summary>
        /// 加载电子书解压后得物理文件夹。
        /// </summary>
        /// <param name="bookId">电子书Id。</param>
        /// <returns>返回电子书组成配置文件实例。</returns>
        IEpubFile Create(string bookId);
    }
}