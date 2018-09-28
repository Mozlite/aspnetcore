using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Mozlite.Extensions.Storages.Excels
{
    /// <summary>
    /// EXCEL管理接口。
    /// </summary>
    public interface IExcelManager : ISingletonService
    {
        /// <summary>
        /// 加载Excel文件。
        /// </summary>
        /// <param name="path">Excel文件物理路径。</param>
        /// <returns>返回Excel内容。</returns>
        IEnumerable<TModel> Load<TModel>(string path)
            where TModel : class, new();

        /// <summary>
        /// 加载Excel文件。
        /// </summary>
        /// <param name="file">表单文件。</param>
        /// <returns>返回Excel内容。</returns>
        Task<IEnumerable<TModel>> LoadAsync<TModel>(IFormFile file)
            where TModel : class, new();

        /// <summary>
        /// 将对象保存到文件中。
        /// </summary>
        /// <typeparam name="TModel">模型列表类型。</typeparam>
        /// <param name="models">模型列表实例。</param>
        /// <param name="path">路径。</param>
        void Save<TModel>(IEnumerable<TModel> models, string path)
            where TModel : class, new();

        /// <summary>
        /// 导出列表。
        /// </summary>
        /// <typeparam name="TModel">当前模型实例类型。</typeparam>
        /// <param name="models">模型列表。</param>
        /// <param name="fileName">文件名称。</param>
        /// <returns>返回物理路径试图结果。</returns>
        PhysicalFileResult Export<TModel>(IEnumerable<TModel> models, string fileName)
            where TModel : class, new();
    }
}