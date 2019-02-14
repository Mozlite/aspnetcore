using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

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

        /// <summary>
        /// 将对象保存到文件中。
        /// </summary>
        /// <param name="path">路径。</param>
        /// <param name="models">模型数据表格。</param>
        /// <param name="sheetName">工作表名称。</param>
        void Save(string path, DataTable models, string sheetName = "sheet1");

        /// <summary>
        /// 导出列表。
        /// </summary>
        /// <param name="models">模型数据表格。</param>
        /// <param name="fileName">文件名称。</param>
        /// <param name="sheetName">工作表名称。</param>
        /// <returns>返回物理路径试图结果。</returns>
        PhysicalFileResult Export(DataTable models, string fileName, string sheetName = "sheet1");
    }
}