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
        /// 发布模板，生成静态页面。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns>返回发布状态。</returns>
        Task<TemplateStatus> ReleaseAsync(string name);
    }
}