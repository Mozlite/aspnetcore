using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Mozlite.Extensions.Storages.Excels
{
    /// <summary>
    /// EXCEL管理类。
    /// </summary>
    public class ExcelManager : IExcelManager
    {
        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// 初始化类<see cref="ExcelManager"/>。
        /// </summary>
        /// <param name="serviceProvider">服务提供者。</param>
        public ExcelManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 加载Excel文件。
        /// </summary>
        /// <param name="path">Excel文件物理路径。</param>
        /// <returns>返回Excel内容。</returns>
        public IEnumerable<TModel> Load<TModel>(string path) where TModel : class, new()
        {
            var model = new ExcelEnumerable<TModel>();
            using (var document = SpreadsheetDocument.Open(path, false))
            {
                var sheet = document.WorkbookPart
                    .WorksheetParts
                    .Select(x => x.Worksheet)
                    .FirstOrDefault();
                if (sheet == null)
                    throw new Exception("Excel中没有存在任何工作表。");
                var shared = document.WorkbookPart.SharedStringTablePart?.SharedStringTable;
                var data = sheet.GetFirstChild<SheetData>();
                model.SheetName = sheet.LocalName;
                Load(model, shared, data);
                return model;
            }
        }

        /// <summary>
        /// 加载Excel文件。
        /// </summary>
        /// <param name="file">表单文件。</param>
        /// <returns>返回Excel内容。</returns>
        public async Task<IEnumerable<TModel>> LoadAsync<TModel>(IFormFile file) where TModel : class, new()
        {
            var storageDirectory = _serviceProvider.GetRequiredService<IStorageDirectory>();
            var info = await storageDirectory.SaveToTempAsync(file);
            var models = Load<TModel>(info.FullName);
            info.Delete();
            return models;
        }

        private void Load<TModel>(ExcelEnumerable<TModel> models, SharedStringTable shared, SheetData sheet) where TModel : class, new()
        {
            var isFirst = true;
            var dic = new Dictionary<string, Action<object, string>>();
            foreach (var row in sheet.Descendants<Row>())
            {
                if (isFirst)
                {//第一行，描述
                    var cells = row.Descendants<Cell>().ToList();
                    foreach (var cell in cells)
                    {
                        var reference = _regex.Replace(cell.CellReference, String.Empty);
                        var descriptor = models.Descriptors.SingleOrDefault(x =>
                            reference.Equals(x.Reference, StringComparison.OrdinalIgnoreCase));
                        if (descriptor == null)
                        {
                            var name = GetCellValue(cell, shared)?.Trim();
                            if (string.IsNullOrWhiteSpace(name))
                                continue;
                            descriptor = models.Descriptors.SingleOrDefault(x =>
                                name.Equals(x.ColumnName, StringComparison.OrdinalIgnoreCase));
                            if (descriptor == null)
                                continue;
                        }
                        dic.Add(reference, descriptor.Set);
                    }
                    isFirst = false;
                }
                else
                {
                    models.Add(Load<TModel>(dic, row, shared));
                }
            }
        }

        private static readonly Regex _regex = new Regex("\\d+");

        private static TModel Load<TModel>(Dictionary<string, Action<object, string>> setters, Row row, SharedStringTable shared) where TModel : class, new()
        {
            var cells = row.Descendants<Cell>().ToList();
            var current = new TModel();
            foreach (var cell in cells)
            {
                if (!setters.TryGetValue(_regex.Replace(cell.CellReference, String.Empty), out var setter))
                    continue;

                var value = GetCellValue(cell, shared);
                if (value != null)
                    setter(current, value);
            }
            return current;
        }

        /// <summary>
        /// 获取当前值。
        /// </summary>
        /// <param name="cell">当前单元格。</param>
        /// <param name="shared">共享字符串表。</param>
        /// <returns>返回当前值。</returns>
        private static string GetCellValue(Cell cell, SharedStringTable shared)
        {
            if (cell.ChildElements.Count == 0)
                return null;

            var value = cell.CellValue.InnerText;
            if (cell.DataType != null && cell.DataType == CellValues.SharedString)
            {
                if (int.TryParse(value, out var index))
                    value = shared.ChildElements[index].InnerText;
                else
                    value = null;
            }

            return value;
        }

        /// <summary>
        /// 将对象保存到文件中。
        /// </summary>
        /// <typeparam name="TModel">模型列表类型。</typeparam>
        /// <param name="models">模型列表实例。</param>
        /// <param name="path">路径。</param>
        public void Save<TModel>(IEnumerable<TModel> models, string path) where TModel : class, new()
        {
            var data = models as ExcelEnumerable<TModel> ?? new ExcelEnumerable<TModel>(models);
            using (var document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
            {
                var index = 1U;
                var workbookPart = document.AddWorkbookPart();
                //写入样式
                WriteStylesheet(workbookPart, data);
                //工作表
                var workSheetPart = workbookPart.AddNewPart<WorksheetPart>();
                var writer = OpenXmlWriter.Create(workSheetPart);
                writer.WriteStartElement(new Worksheet());
                writer.WriteStartElement(new SheetData());
                //字段定义
                var descriptors = data.Descriptors.OrderBy(x => x.Index).ToList();
                var sharedStrings = new List<string>();
                //第一行标题
                var row = new Row();
                row.RowIndex = index;
                for (var i = 0; i < data.Columns; i++)
                {
                    var descriptor = descriptors[i];
                    var cell = new Cell();
                    cell.StyleIndex = descriptor.HeadCellFormat.FormatId;
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(descriptor.ColumnName);
                    cell.CellReference = $"{(char)('A' + i)}{index}";
                    row.AppendChild(cell);
                }
                writer.WriteElement(row);

                index++;
                //写入数据
                foreach (var model in data)
                {
                    row = new Row();
                    row.RowIndex = index;
                    for (var i = 0; i < data.Columns; i++)
                    {
                        var descriptor = descriptors[i];
                        var value = descriptor.Get(model);
                        if (value == null)
                            continue;
                        var type = CellValues.Error;
                        if (value is DateTime date)
                            value = date.ToOADate();
                        else if (value is DateTimeOffset dateTimeOffset)
                            value = dateTimeOffset.DateTime.ToOADate();
                        else if (value is bool bValue)
                        {
                            value = bValue ? 1 : 0;
                            type = CellValues.Boolean;
                        }
                        else if (!value.GetType().IsValueType)
                        {
                            type = CellValues.SharedString;
                            var current = value.ToString();
                            var si = sharedStrings.IndexOf(current);
                            if (si == -1)
                            {
                                si = sharedStrings.Count;
                                sharedStrings.Add(current);
                            }
                            value = si;
                        }

                        var cell = new Cell();
                        cell.StyleIndex = descriptor.CellFormat.FormatId;
                        if (type != CellValues.Error)
                            cell.DataType = type;
                        cell.CellReference = $"{(char)('A' + i)}{index}";
                        cell.CellValue = new CellValue(value.ToString());
                        row.AppendChild(cell);
                    }
                    writer.WriteElement(row);
                    index++;
                }

                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.Close();
                //工作区
                writer = OpenXmlWriter.Create(document.WorkbookPart);
                writer.WriteStartElement(new Workbook());
                writer.WriteStartElement(new Sheets());
                writer.WriteElement(new Sheet
                {
                    Name = data.SheetName,
                    SheetId = 1,
                    Id = document.WorkbookPart.GetIdOfPart(workSheetPart)
                });
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.Close();

                //写入字符串
                var shared = workbookPart.AddNewPart<SharedStringTablePart>();
                var table = new SharedStringTable();
                foreach (var sharedString in sharedStrings)
                {
                    table.AppendChild(new SharedStringItem(new Text(sharedString)));
                }
                table.Save(shared);
            }
        }

        /// <summary>
        /// 需要先写入样式，可以得到没列样式Id。
        /// </summary>
        private void WriteStylesheet<TModel>(WorkbookPart workbookPart, ExcelEnumerable<TModel> models) where TModel : class, new()
        {
            var stylesheet = new Stylesheet();
            stylesheet.Fonts = new Fonts();//第一个默认
            stylesheet.Fonts.AppendChild(new Font());
            stylesheet.CellFormats = new CellFormats();
            stylesheet.CellFormats.AppendChild(new CellFormat());
            stylesheet.NumberingFormats = new NumberingFormats();
            stylesheet.Borders = new Borders();
            stylesheet.CellStyleFormats = new CellStyleFormats();
            stylesheet.Fills = new Fills();

            var fontIndex = 1U;//默认字体啥都没有
            var formatId = 1U;//格式Id
            var numberId = 1U;//数值格式
            foreach (var descriptor in models.Descriptors)
            {
                descriptor.CellFormat.FormatId = formatId++;
                stylesheet.CellFormats.AppendChild(descriptor.CellFormat);
                descriptor.HeadCellFormat.FormatId = formatId++;
                stylesheet.CellFormats.AppendChild(descriptor.HeadCellFormat);

                if (descriptor.Font != null)
                {
                    stylesheet.Fonts.AppendChild(descriptor.Font);
                    descriptor.CellFormat.FontId = fontIndex;
                    descriptor.CellFormat.ApplyFont = true;
                    fontIndex++;
                }

                if (descriptor.HeadFont != null)
                {
                    stylesheet.Fonts.AppendChild(descriptor.HeadFont);
                    descriptor.HeadCellFormat.FontId = fontIndex;
                    descriptor.HeadCellFormat.ApplyFont = true;
                    fontIndex++;
                }

                if (descriptor.NumberingFormat != null)
                {
                    stylesheet.NumberingFormats.AppendChild(descriptor.NumberingFormat);
                    descriptor.NumberingFormat.NumberFormatId = numberId;
                    descriptor.CellFormat.NumberFormatId = numberId;
                    descriptor.CellFormat.ApplyNumberFormat = true;
                    numberId++;
                }
            }

            stylesheet.CellFormats.Count = (uint)stylesheet.CellFormats.ChildElements.Count;
            stylesheet.Fonts.Count = (uint)stylesheet.Fonts.ChildElements.Count;
            stylesheet.NumberingFormats.Count = (uint)stylesheet.NumberingFormats.ChildElements.Count;
            stylesheet.Borders.Count = (uint)stylesheet.Borders.ChildElements.Count;
            stylesheet.CellStyleFormats.Count = (uint)stylesheet.CellStyleFormats.ChildElements.Count;
            stylesheet.Fills.Count = (uint)stylesheet.Fills.ChildElements.Count;
            var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
            stylesheet.Save(stylesPart);
        }

        /// <summary>
        /// 导出列表。
        /// </summary>
        /// <typeparam name="TModel">当前模型实例类型。</typeparam>
        /// <param name="models">模型列表。</param>
        /// <param name="fileName">文件名称。</param>
        /// <returns>返回物理路径试图结果。</returns>
        public PhysicalFileResult Export<TModel>(IEnumerable<TModel> models, string fileName) where TModel : class, new()
        {
            const string extension = ".xlsx";
            if (!fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                fileName += extension;
            var storageDirectory = _serviceProvider.GetRequiredService<IStorageDirectory>();
            var excels = new ExcelEnumerable<TModel>(models);
            var path = storageDirectory.GetTempPath(Guid.NewGuid().ToString("N"));
            Save(excels, path);
            var result = new PhysicalFileResult(path, extension.GetContentType());
            result.FileDownloadName = fileName;
            return result;
        }
    }
}