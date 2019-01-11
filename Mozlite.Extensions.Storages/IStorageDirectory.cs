using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mozlite.Extensions.Storages
{
    /// <summary>
    /// 存储文件夹提供者接口。
    /// </summary>
    public interface IStorageDirectory : ISingletonService
    {
        /// <summary>
        /// 获取当前相对路径的物理存储路径。
        /// </summary>
        /// <param name="path">相对路径，以子文件夹或文件开始。</param>
        /// <returns>返回当前相对路径的物理路径。</returns>
        string GetPhysicalPath(string path = null);

        /// <summary>
        /// 获取临时目录得物理路径。
        /// </summary>
        /// <param name="fileName">文件名称。</param>
        /// <returns>返回当前临时文件物理路径。</returns>
        string GetTempPath(string fileName = null);

        /// <summary>
        /// 获取当前路径文件的操作提供者接口实例。
        /// </summary>
        /// <param name="path">文件相对路径。</param>
        /// <returns>文件的操作提供者接口实例。</returns>
        IStorageFile GetFile(string path);

        /// <summary>
        /// 将表单文件实例保存到临时文件夹中。
        /// </summary>
        /// <param name="file">表单文件实例。</param>
        /// <returns>返回文件实例。</returns>
        Task<FileInfo> SaveToTempAsync(IFormFile file);

        /// <summary>
        /// 将表单文件实例保存到临时文件夹中。
        /// </summary>
        /// <param name="file">表单文件实例。</param>
        /// <returns>返回文件实例。</returns>
        Task<FileInfo> SaveToTempAsync(Stream file);

        /// <summary>
        /// 将字符串保存到临时文件夹中。
        /// </summary>
        /// <param name="text">要保存的字符串。</param>
        /// <param name="fileName">文件名。</param>
        /// <returns>返回文件实例。</returns>
        Task<FileInfo> SaveToTempAsync(string text, string fileName = null);

        /// <summary>
        /// 将表单文件实例保存到特定的文件夹中。
        /// </summary>
        /// <param name="file">表单文件实例。</param>
        /// <param name="directoryName">文件夹名称。</param>
        /// <param name="fileName">文件名称，如果为空，则直接使用表单实例的文件名。</param>
        /// <returns>返回文件提供者操作接口实例。</returns>
        Task<IStorageFile> SaveAsync(IFormFile file, string directoryName, string fileName = null);

        /// <summary>
        /// 将字符串保存到特定的文件夹中。
        /// </summary>
        /// <param name="text">要保存的字符串。</param>
        /// <param name="directoryName">文件夹名称。</param>
        /// <param name="fileName">文件名称，如果为空，则直接使用表单实例的文件名。</param>
        /// <returns>返回文件提供者操作接口实例。</returns>
        Task<IStorageFile> SaveAsync(string text, string directoryName, string fileName);

        /// <summary>
        /// 清理空文件夹。
        /// </summary>
        void ClearEmptyDirectories();
    }
}