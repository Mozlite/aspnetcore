using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mozlite.Extensions.Storages
{
    /// <summary>
    /// 媒体文件提供者接口。
    /// </summary>
    public interface IMediaDirectory : ISingletonService
    {
        /// <summary>
        /// 上传文件。
        /// </summary>
        /// <param name="file">表单文件。</param>
        /// <param name="init">实例化媒体文件属性。</param>
        /// <param name="unique">每一个文件和媒体存储文件一一对应。</param>
        /// <returns>返回上传后的结果！</returns>
        Task<MediaResult> UploadAsync(IFormFile file, Action<MediaFile> init, bool unique = true);

        /// <summary>
        /// 上传文件。
        /// </summary>
        /// <param name="file">表单文件。</param>
        /// <param name="extensionName">扩展名称。</param>
        /// <param name="targetId">目标Id。</param>
        /// <param name="unique">每一个文件和媒体存储文件一一对应。</param>
        /// <returns>返回上传后的结果！</returns>
        Task<MediaResult> UploadAsync(IFormFile file, string extensionName, int? targetId = null, bool unique = true);

        /// <summary>
        /// 下载文件。
        /// </summary>
        /// <param name="url">文件URL地址。</param>
        /// <param name="init">实例化媒体文件属性。</param>
        /// <param name="unique">每一个文件和媒体存储文件一一对应。</param>
        /// <returns>返回上传后的结果！</returns>
        Task<MediaResult> DownloadAsync(string url, Action<MediaFile> init, bool unique = true);

        /// <summary>
        /// 下载文件。
        /// </summary>
        /// <param name="url">文件URL地址。</param>
        /// <param name="extensionName">扩展名称。</param>
        /// <param name="targetId">目标Id。</param>
        /// <param name="unique">每一个文件和媒体存储文件一一对应。</param>
        /// <returns>返回上传后的结果！</returns>
        Task<MediaResult> DownloadAsync(string url, string extensionName, int? targetId = null, bool unique = true);

        /// <summary>
        /// 通过GUID获取存储文件实例。
        /// </summary>
        /// <param name="id">媒体文件Id。</param>
        /// <returns>返回存储文件实例。</returns>
        Task<StoredPhysicalFile> FindAsync(Guid id);

        /// <summary>
        /// 通过扩展名称和目标Id。
        /// </summary>
        /// <param name="extensionName">扩展名称。</param>
        /// <param name="targetId">目标Id。</param>
        /// <returns>返回媒体文件。</returns>
        Task<MediaFile> FindAsync(string extensionName, int targetId);

        /// <summary>
        /// 通过扩展名称和目标Id。
        /// </summary>
        /// <param name="extensionName">扩展名称。</param>
        /// <param name="targetId">目标Id。</param>
        /// <returns>返回媒体文件列表。</returns>
        Task<IEnumerable<MediaFile>> FetchAsync(string extensionName, int targetId);

        /// <summary>
        /// 加载文件。
        /// </summary>
        /// <param name="query">查询实例。</param>
        /// <returns>返回文件列表。</returns>
        Task<MediaQuery> LoadAsync(MediaQuery query);

        /// <summary>
        /// 删除文件。
        /// </summary>
        /// <param name="id">文件Id。</param>
        /// <returns>返回删除结果。</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// 删除文件。
        /// </summary>
        /// <param name="extensionName">扩展名称。</param>
        /// <param name="targetId">对象Id。</param>
        /// <returns>返回删除结果。</returns>
        Task<bool> DeleteAsync(string extensionName, int? targetId =null);
    }
}