using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mozlite.Extensions.Storages
{
    /// <summary>
    /// 存储文件夹提供者接口。
    /// </summary>
    public interface IStorageDirectoryProvider : ISingletonService
    {
        /// <summary>
        /// 获取当前路径的物理路径。
        /// </summary>
        /// <param name="path">当前相对路径。</param>
        /// <returns>返回当前路径的物理路径。</returns>
        string PhysicalPath(string path);

        /// <summary>
        /// 获取当前路径文件的操作提供者接口实例。
        /// </summary>
        /// <param name="path">文件相对路径。</param>
        /// <returns>文件的操作提供者接口实例。</returns>
        IStorageFileProvider GetFile(string path);

        /// <summary>
        /// 将表单文件实例保存到特定的文件夹中。
        /// </summary>
        /// <param name="file">表单文件实例。</param>
        /// <param name="directoryName">文件夹名称。</param>
        /// <param name="fileName">文件名称，如果为空，则直接使用表单实例的文件名。</param>
        /// <returns>返回文件提供者操作接口实例。</returns>
        Task<IStorageFileProvider> SaveAsync(IFormFile file, string directoryName, string fileName = null);
    }
}