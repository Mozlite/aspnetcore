using System;
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
        /// <param name="extensionName">扩展名称。</param>
        /// <param name="targetId">目标Id。</param>
        /// <param name="uniqueMediaFile">每一个文件和媒体存储文件一一对应。</param>
        /// <returns>返回上传后的结果！</returns>
        Task<MediaResult> UploadAsync(IFormFile file, string extensionName, int? targetId = null, bool uniqueMediaFile = true);

        /// <summary>
        /// 下载文件。
        /// </summary>
        /// <param name="url">文件URL地址。</param>
        /// <param name="extensionName">扩展名称。</param>
        /// <param name="targetId">目标Id。</param>
        /// <param name="uniqueMediaFile">每一个文件和媒体存储文件一一对应。</param>
        /// <returns>返回上传后的结果！</returns>
        Task<MediaResult> DownloadAsync(string url, string extensionName, int? targetId = null, bool uniqueMediaFile = true);

        /// <summary>
        /// 通过GUID获取存储文件实例。
        /// </summary>
        /// <param name="id">媒体文件Id。</param>
        /// <returns>返回存储文件实例。</returns>
        Task<StoredPhysicalFile> FindAsync(Guid id);

        /// <summary>
        /// 加载文件。
        /// </summary>
        /// <param name="query">查询实例。</param>
        /// <returns>返回文件列表。</returns>
        Task<MediaQuery> LoadAsync(MediaQuery query);
    }
}