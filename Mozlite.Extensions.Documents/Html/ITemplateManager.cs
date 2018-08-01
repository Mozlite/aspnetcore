using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mozlite.Extensions.Html
{
    /// <summary>
    /// 模板接口。
    /// </summary>
    public interface ITemplateManager : ISingletonService
    {
        /// <summary>
        /// 安装模板，使用zip压缩。
        /// </summary>
        /// <param name="file">上传得文件实例。</param>
        /// <returns>返回状态。</returns>
        Task<TemplateStatus> SetupAsync(IFormFile file);

        /// <summary>
        /// 获取配置实例。
        /// </summary>
        /// <returns>返回配置实例。</returns>
        Task<IEnumerable<TemplateConfiguration>> LoadTemplatesAsync();

        /// <summary>
        /// 获取配置实例。
        /// </summary>
        /// <returns>返回配置实例。</returns>
        Task<TemplateConfiguration> GetTemplateAsync(Guid id);

        /// <summary>
        /// 获取模板物理路径。
        /// </summary>
        /// <param name="id">模板Id。</param>
        /// <returns>返回当前目录物理路径。</returns>
        string GetTemplatePath(Guid id);
    }
}