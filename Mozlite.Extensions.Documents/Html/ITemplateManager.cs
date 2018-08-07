using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mozlite.Extensions.Documents.Html
{
    /// <summary>
    /// 模板接口。
    /// </summary>
    public interface ITemplateManager : IScopedService
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

        /// <summary>
        /// 生成HTML并保存到文件中。
        /// </summary>
        /// <param name="path">路径。</param>
        /// <param name="configuration">配置实例。</param>
        /// <param name="outputTemplate">保存文件路径模板，可以用<paramref name="routeData"/>键来配置模板，如：{page}等。</param>
        /// <param name="routeData">路由实例。</param>
        Task SaveGeneratorAsync(string path, TemplateConfiguration configuration, string outputTemplate = null, IDictionary<string, object> routeData = null);
    }
}